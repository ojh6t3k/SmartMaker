package com.smartmaker.android;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.util.UUID;
import java.util.Set;
import java.util.ArrayList;
import java.util.List;

import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothSocket;
import android.content.Context;


public class CommBluetooth
{
	// Member fields
    private static CommBluetooth _Instance = null;
    private Context _context;
	private BluetoothAdapter mAdapter;
	private ConnectThread mConnectThread;
	private ConnectedThread mConnectedThread;
    private boolean _isOpening = false;
	private boolean _isOpened = false;
	private static final UUID MY_UUID = UUID.fromString("00001101-0000-1000-8000-00805F9B34FB");
	private byte[] _rcvBuffer = new byte[4096];
	private int _numRcvedData = 0;

    public static CommBluetooth GetInstance()
    {
        if(_Instance == null)
            _Instance = new CommBluetooth();

        return _Instance;
    }

    public void SetContext(Context context)
    {
        _context = context;
        mAdapter = BluetoothAdapter.getDefaultAdapter();
    }

    public synchronized String[] DeviceSearch()
    {
        List<String> devNames = new ArrayList<String>();

        try
        {
            Set<BluetoothDevice> pairedDevices = mAdapter.getBondedDevices();
            if (pairedDevices.size() > 0)
            {
                for (BluetoothDevice bd : pairedDevices)
                    devNames.add(bd.getName());
            }
        }
        catch (Exception e)
        {
        }

        return devNames.toArray(new String[devNames.size()]);
    }

    public boolean IsOpening()
    {
        return _isOpening;
    }

	public boolean IsOpen()
	{
		return _isOpened;
	}
	
	public synchronized boolean Open(String deviceName)
	{
        Set<BluetoothDevice> pairedDevices = mAdapter.getBondedDevices();
        for (BluetoothDevice bd : pairedDevices)
        {
            if (bd.getName().equalsIgnoreCase(deviceName))
            {
        		// Cancel any thread currently running a connection
        		if (mConnectedThread != null)
        		{
        			mConnectedThread.cancel();
        			mConnectedThread = null;
        		}

        		// Start the thread to connect with the given device
        		mConnectThread = new ConnectThread(bd);
        		mConnectThread.start();
                _isOpened = false;
                _isOpening = true;
                return true;
            }
        }

        return false;
	}
	
	public synchronized void Close()
	{
        _isOpened = false;
        _isOpening = false;
		
		if (mConnectThread != null)
		{
			mConnectThread.cancel();
			mConnectThread = null;
		}
		if (mConnectedThread != null)
		{
			mConnectedThread.cancel();
			mConnectedThread = null;
		}		
	}
	
	public void ClearBuffer()
	{
		_numRcvedData = 0;
	}	

	private synchronized void Connected(BluetoothSocket socket,	BluetoothDevice device)
	{
		// Cancel the thread that completed the connection
		if (mConnectThread != null)
		{
			mConnectThread.cancel();
			mConnectThread = null;
		}

		// Cancel any thread currently running a connection
		if (mConnectedThread != null)
		{
			mConnectedThread.cancel();
			mConnectedThread = null;
		}

		// Start the thread to manage the connection and perform transmissions
		mConnectedThread = new ConnectedThread(socket);
		mConnectedThread.start();
		
		ClearBuffer();
        _isOpened = true;
	}

	public void Write(byte[] data)
	{
		// Create temporary object
		ConnectedThread r;
		// Synchronize a copy of the ConnectedThread
		synchronized (this)
		{
			if (_isOpened == false)
				return;
			r = mConnectedThread;
		}
		// Perform the write unsynchronized
		r.write(data);
	}
	
	public byte[] Read()
	{
		if (_isOpened == false)
			return null;
		
		byte[] data = new byte[_numRcvedData];
		for(int i=0; i<_numRcvedData; i++)
			data[i] = _rcvBuffer[i];
		_numRcvedData = 0;
		return data;
	}
	
	private class ConnectThread extends Thread
	{
		private final BluetoothSocket mmSocket;
		private final BluetoothDevice mmDevice;

		public ConnectThread(BluetoothDevice device)
		{
			mmDevice = device;
			BluetoothSocket tmp = null;

			// Get a BluetoothSocket for a connection with the
			// given BluetoothDevice
			try
			{
				tmp = device.createRfcommSocketToServiceRecord(MY_UUID);
			}
			catch (IOException e)
			{
			}
			mmSocket = tmp;
		}

		public void run()
		{
			setName("ConnectThread");

			// Make a connection to the BluetoothSocket
			try
			{
				mmSocket.connect();
			}
			catch (IOException e)
			{
                _isOpening = false;

				// Close the socket
				try
				{
					mmSocket.close();
				}
				catch (IOException e2)
				{
				}
				return;
			}

			// Reset the ConnectThread because we're done
			synchronized (CommBluetooth.this)
			{
				mConnectThread = null;
			}

			// Start the connected thread
			Connected(mmSocket, mmDevice);
		}

		public void cancel()
		{
			try
			{
				mmSocket.close();
			}
			catch (IOException e)
			{
			}
		}
	}

	private class ConnectedThread extends Thread
	{
		private final BluetoothSocket mmSocket;
		private final InputStream mmInStream;
		private final OutputStream mmOutStream;

		public ConnectedThread(BluetoothSocket socket)
		{
			mmSocket = socket;
			InputStream tmpIn = null;
			OutputStream tmpOut = null;

			// Get the BluetoothSocket input and output streams
			try
			{
				tmpIn = socket.getInputStream();
				tmpOut = socket.getOutputStream();
			}
			catch (IOException e)
			{
			}

			mmInStream = tmpIn;
			mmOutStream = tmpOut;
		}
		
		public void run()
		{
            int bytes;

            // Keep listening to the InputStream while connected
            while (true)
            {
                try
                {
                	bytes = mmInStream.available();
                	if(bytes > 0)
                	{
	                	if(bytes <= (_rcvBuffer.length - _numRcvedData))
	                	{
	                		mmInStream.read(_rcvBuffer, _numRcvedData, bytes);
	                		_numRcvedData += bytes;
	                	}
                	}
                }
                catch (IOException e)
                {
                	if(_isOpened == true)
                	{
                        _isOpened = false;
                	}
                    break;
                }
            }
		}

		public void write(byte[] buffer)
		{
			try
			{
				mmOutStream.write(buffer);
			}
			catch (IOException e)
			{
			}
		}

		public void cancel()
		{
			try
			{
				mmSocket.close();
			}
			catch (IOException e)
			{
			}
		}
	}
}
