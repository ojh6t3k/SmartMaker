/*
  NeoPixel.cpp - SmartMaker library
  Copyright (C) 2015 ojh6t3k.  All rights reserved.
*/

//******************************************************************************
//* Includes
//******************************************************************************
#include "UnityApp.h"
#include "NeoPixel.h"


//******************************************************************************
//* Constructors
//******************************************************************************

NeoPixel::NeoPixel(int id, int num, int pin, int config) : AppAction(id)
{
	_strip = Adafruit_NeoPixel(num, pin, config);
}


//******************************************************************************
//* Override Methods
//******************************************************************************
void AnalogInput::OnSetup()
{
	_strip.begin();
	_strip.show();
}

void AnalogInput::OnStart()
{
}

void AnalogInput::OnStop()
{	
}

void AnalogInput::OnProcess()
{	
}

void AnalogInput::OnUpdate()
{
	UnityApp.pop(&_index);
	UnityApp.pop(&_red);
	UnityApp.pop(&_green);
	UnityApp.pop(&_blue);
}

void AnalogInput::OnExcute()
{
	_strip.setPixelColor(_index, _red, _green, _blue);
	_strip.show();
}

void AnalogInput::OnFlush()
{
}
