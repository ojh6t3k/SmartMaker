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
void NeoPixel::OnSetup()
{
	_strip.begin();
	_strip.show();
}

void NeoPixel::OnStart()
{
}

void NeoPixel::OnStop()
{	
}

void NeoPixel::OnProcess()
{	
}

void NeoPixel::OnUpdate()
{
	UnityApp.pop(&_index);
	UnityApp.pop(&_red);
	UnityApp.pop(&_green);
	UnityApp.pop(&_blue);
	UnityApp.pop(&_brightness);
}

void NeoPixel::OnExcute()
{
	_strip.setBrightness(_brightness);
	_strip.setPixelColor(_index, _red, _green, _blue);
	_strip.show();
}

void NeoPixel::OnFlush()
{
}
