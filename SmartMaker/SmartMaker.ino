#include "UnityApp.h"
#include "DigitalOutput.h"

DigitalOutput dOutput0(0, 13);

void setup()
{
  UnityApp.attachAction((AppAction*)&dOutput0);
  UnityApp.begin(115200);
}

void loop()
{
  UnityApp.process();
}
