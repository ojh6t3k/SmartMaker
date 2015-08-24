/*
  NeoPixel.h - SmartMaker library
  Copyright (C) 2015 ojh6t3k.  All rights reserved.

  This class is included Adafruit_NeoPixel.
*/

#ifndef NeoPixel_h
#define NeoPixel_h

#if (ARDUINO >= 100)
 #include <Arduino.h>
#else
 #include <WProgram.h>
 #include <pins_arduino.h>
#endif

#include "AppAction.h"


// 'type' flags for LED pixels (third parameter to constructor):
#define NEO_RGB     0x00 // Wired for RGB data order
#define NEO_GRB     0x01 // Wired for GRB data order
#define NEO_COLMASK 0x01
#define NEO_KHZ400  0x00 // 400 KHz datastream
#define NEO_KHZ800  0x02 // 800 KHz datastream
#define NEO_SPDMASK 0x02


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
	byte _index;
	byte _red;
	byte _green;
	byte _blue;
	byte _brightness;

	// Adafruit_NeoPixel Class
	const uint16_t numLEDs;       // Number of RGB LEDs in strip
	const uint16_t numBytes;      // Size of 'pixels' buffer below
	const uint8_t _pin;           // Output pin number
	const uint8_t type;          // Pixel flags (400 vs 800 KHz, RGB vs GRB color)
	uint8_t brightness;
	uint8_t *pixels;        // Holds LED color values (3 bytes each)
	uint32_t endTime;       // Latch timing reference
#ifdef __AVR__
	const volatile uint8_t *port;         // Output PORT register
	const uint8_t pinMask;       // Output PORT bitmask
#endif

	void begin(void);
    void show(void);
    void setPixelColor(uint16_t n, uint8_t r, uint8_t g, uint8_t b);
    void setPixelColor(uint16_t n, uint32_t c);
    void setBrightness(uint8_t);
	uint16_t numPixels(void);
	static uint32_t Color(uint8_t r, uint8_t g, uint8_t b);
	uint32_t getPixelColor(uint16_t n);
};

#endif

