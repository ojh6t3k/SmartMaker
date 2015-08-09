/*
  NeoPixel.h - SmartMaker library
  Copyright (C) 2015 ojh6t3k.  All rights reserved.
*/

#ifndef NeoPixel_h
#define NeoPixel_h

#include <Adafruit_NeoPixel.h>
#include "AppAction.h"


class NeoPixel : AppAction
{
public:
	NeoPixel(int id, int num, int pin, int config);

protected:
	void OnSetup();
	void OnStart();
	void OnStop();
	void OnProcess();
	void OnUpdate();
	void OnExcute();
	void OnFlush();

private:
	Adafruit_NeoPixel _strip;
	byte _index;
	byte _red;
	byte _green;
	byte _blue;
	byte _brightness;
};

#endif

