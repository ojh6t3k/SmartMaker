package com.smartmaker.android;

import android.content.Context;
import android.os.Vibrator;


public class Vibration
{
	private static Vibration _Instance = null;
    private Context _context;
    private Vibrator _vibeService = null;
    
    public static Vibration GetInstance()
    {
        if(_Instance == null)
            _Instance = new Vibration();

        return _Instance;
    }
    
    public boolean Initialize(Context context)
    {       
        _vibeService = (Vibrator)context.getSystemService(Context.VIBRATOR_SERVICE);
        if(_vibeService == null)
        	return false;
        
        _context = context;
        return true;
    }
    
    public void Vibrate(int milliseconds)
    {
    	if(_vibeService != null)
    		_vibeService.vibrate(milliseconds);
    }
}
