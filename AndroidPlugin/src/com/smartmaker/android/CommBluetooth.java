package com.smartmaker.android;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.util.UUID;
import java.util.Set;
import java.util.ArrayList;
import java.util.List;

import android.annotation.SuppressLint;
import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothSocket;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.util.Log;

import com.unity3d.player.UnityPlayer;


@SuppressLint("NewApi")
public class CommBluetooth
{
	// Member fields
    private static CommBluetooth _Instance = null;
    private static String _logTag = "CommBluetooth";
    private Context _context;
    private static final UUID SPP_UUID = UUID.fromString("00001101-0000-1000-8000-00805F9B34FB");
	private BluetoothAdapter _btAdapter;
	private BluetoothDevice _btDevice;
	private BluetoothSocket _btSocket;
	private InputStream _InStream;
	private OutputStream _OutStream;
	private boolean _isOpen = false;
	private String _unityObject;
	private String _unityMethodOpenSuccess;
	private String _unityMethodOpenFailed;
	private String _unityMethodErrorClose;
	private String _unityMethodFoundDevice;
	private boolean _isErrorClose = false;
	private String _errorMessage;

    public static CommBluetooth GetInstance()
    {
    	Log.d(_logTag, "GetInstance");
    	
        if(_Instance == null)
            _Instance = new CommBluetooth();

        return _Instance;
    }

    public boolean Initialize(Context context, String unityObject)
    {
		Log.d(_logTag, "Initialize");
		
      	_btAdapter = BluetoothAdapter.getDefaultAdapter();
        if(_btAdapter == null)
        {
        	Log.d(_logTag, "Bluetooth Adapter Failed");
        	return false;
        }
        
        if(!_btAdapter.isEnabled())
        	context.startActivity(new Intent(BluetoothAdapter.ACTION_REQUEST_ENABLE));
		
        _context = context;
        _unityObject = unityObject;        
        _btDevice = null;
        _btSocket = null;
        _InStream = null;
        _OutStream = null;
        
        final IntentFilter intentFilter = new IntentFilter();
		intentFilter.addAction(BluetoothDevice.ACTION_FOUND);
		intentFilter.addAction(BluetoothDevice.ACTION_ACL_CONNECTED);
		intentFilter.addAction(BluetoothDevice.ACTION_ACL_DISCONNECTED);
		intentFilter.addAction(BluetoothDevice.ACTION_ACL_DISCONNECT_REQUESTED);
        _context.registerReceiver(_bluetoothReceiver, intentFilter);
        _context.registerReceiver(_bluetoothReceiver, new IntentFilter(BluetoothAdapter.ACTION_DISCOVERY_FINISHED));

        return true;
    }
    
    public void SetUnityMethodOpenSuccess(String unityMethod)
	{
		_unityMethodOpenSuccess = unityMethod;
	}
	
	public void SetUnityMethodOpenFailed(String unityMethod)
	{
		_unityMethodOpenFailed = unityMethod;
	}
	
	public void SetUnityMethodErrorClose(String unityMethod)
	{
		_unityMethodErrorClose = unityMethod;
	}
	
	public void SetUnityMethodFoundDevice(String unityMethod)
	{
		_unityMethodFoundDevice = unityMethod;
	}

	public synchronized String[] GetBondedDevices()
    {
		Log.d(_logTag, "GetBondedDevices");
		
        List<String> btDevices = new ArrayList<String>();
        
        if(_btAdapter.isEnabled())
        {
        	try
            {
                Set<BluetoothDevice> bondedDevices = _btAdapter.getBondedDevices();
                if (bondedDevices.size() > 0)
                {
                    for (BluetoothDevice bd : bondedDevices)
                   		btDevices.add(String.format("%s,%s", bd.getName(), bd.getAddress()));
                }
            }
            catch (Exception e)
            {
            }
        }
        else
            _context.startActivity(new Intent(BluetoothAdapter.ACTION_REQUEST_ENABLE));
        
        return btDevices.toArray(new String[btDevices.size()]);
    }

	public void StartSearch()
	{
		Log.d(_logTag, "StartSearch");
		
		if(_btAdapter == null)
			return;
		
		_btAdapter.startDiscovery();
	}
	
	public void StopSearch()
	{
		Log.d(_logTag, "StopSearch");
		
		if(_btAdapter == null)
			return;
		
		if (_btAdapter.isDiscovering())
			_btAdapter.cancelDiscovery();
	}
	
	public synchronized void Open(String address)
	{
		Log.d(_logTag, "Open");
		
		if(_btAdapter.isEnabled())
		{			
			try
			{
				_btDevice = _btAdapter.getRemoteDevice(address);
				_btSocket = _btDevice.createRfcommSocketToServiceRecord(SPP_UUID);
				_btSocket.connect();				
			}
			catch (IOException e)
			{
				Log.d(_logTag, "Open Failed");
				close();
				UnityPlayer.UnitySendMessage(_unityObject, _unityMethodOpenFailed, "Open Failed");
			}
		}
		else
		{
			Log.d(_logTag, "Bluetooth Disabled");
			
			_context.startActivity(new Intent(BluetoothAdapter.ACTION_REQUEST_ENABLE));
			UnityPlayer.UnitySendMessage(_unityObject, _unityMethodOpenFailed, "Bluetooth Disabled");
		}
	}
	
	public synchronized void Close()
	{
		Log.d(_logTag, "Close");
		
		close();
	}
	
	public boolean IsOpen()
	{
		return _isOpen;
	}
	
	public synchronized void Write(byte[] data)
	{
		if(_isOpen)
		{
			if(_OutStream != null)
			{
				try
				{
					_OutStream.write(data);
				}
				catch (Exception e)
				{
					Log.d(_logTag, "Write Error");
					_isErrorClose = true;
					_errorMessage = "Write Error";
					close();
				}
			}
		}
	}
	
	public int Available()
	{
		if(_isOpen)
		{
			if(_InStream != null)
			{
				try
				{
					return _InStream.available();
				}
				catch (Exception e)
				{
					Log.d(_logTag, "Avaliable Error");
					_isErrorClose = true;
					_errorMessage = "Avaliable Error";
					close();
				}
			}
			else
			{
				
			}
		}
		
		return 0;
	}
	
	public synchronized byte[] Read()
	{
		if(_isOpen)
		{
			if(_InStream != null)
			{
				try
				{
					byte[] data = new byte[_InStream.available()];
					_InStream.read(data);
					return data;
				}
				catch (Exception e)
				{
					Log.d(_logTag, "Read Error");
					_isErrorClose = true;
					_errorMessage = "Read Error";
					close();
				}
			}
			else
			{
				
			}
		}
		
		return null;
	}
	
	private void close()
	{
		if(!_isOpen)
			return;
		
		_isOpen = false;
		
		if(_btSocket != null)
		{
			try
			{
				_btSocket.close();
			}
			catch(Exception e)
			{	
			}
			_btSocket = null;
		}
		
		_btDevice = null;
		_InStream = null;
		_OutStream = null;
	}
	
	private final BroadcastReceiver _bluetoothReceiver = new BroadcastReceiver()
	{
		@Override
		public void onReceive(Context context, Intent intent)
		{
			String action = intent.getAction();

			if (BluetoothDevice.ACTION_FOUND.equalsIgnoreCase(action))
			{
				BluetoothDevice device = intent.getParcelableExtra(BluetoothDevice.EXTRA_DEVICE);
				Log.d(_logTag, "Bluetooth Device Found: " + device.getName());
				
				UnityPlayer.UnitySendMessage(_unityObject, _unityMethodFoundDevice, String.format("%s,%s", device.getName(), device.getAddress()));			
			}
			else if (BluetoothAdapter.ACTION_DISCOVERY_FINISHED.equalsIgnoreCase(action))
			{
				Log.d(_logTag, "ACTION_DISCOVERY_FINISHED");
			}
			else if (BluetoothDevice.ACTION_ACL_CONNECTED.equalsIgnoreCase(action))
			{
				Log.d(_logTag, "Bluetooth Device Connected!");
				
				try
				{
					_InStream = _btSocket.getInputStream();
					_OutStream = _btSocket.getOutputStream();
					_isOpen = true;
					UnityPlayer.UnitySendMessage(_unityObject, _unityMethodOpenSuccess, "Connected");
				}
				catch(Exception e)
				{
					Log.d(_logTag, "Open Failed");
					close();
					UnityPlayer.UnitySendMessage(_unityObject, _unityMethodOpenFailed, "Open Failed");
				}				
			}
			else if (BluetoothDevice.ACTION_ACL_DISCONNECTED.equalsIgnoreCase(action) || BluetoothDevice.ACTION_ACL_DISCONNECT_REQUESTED.equalsIgnoreCase(action))
			{
				Log.d(_logTag, "Device Disconnected!!");
				if(_isOpen)
				{
					close();
					_isErrorClose = true;
					_errorMessage = "Device Disconnected";
				}
				
				if(_isErrorClose)
				{
					UnityPlayer.UnitySendMessage(_unityObject, _unityMethodErrorClose, _errorMessage);
					_isErrorClose = false;
					_errorMessage = "";
				}
			}
			else
			{
				Log.d(_logTag, "UNKNOWN ACTION : " + action);
			}
		}
	};
}
