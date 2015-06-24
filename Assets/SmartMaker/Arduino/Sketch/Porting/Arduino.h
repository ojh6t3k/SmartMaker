/*
  Arduino.h - Arduino ������ ���� �ҽ�
*/

#ifndef Arduino_h
#define Arduino_h

//-----------------------------------------------------------//
// ����, �Ʒ� ������ ���� ȯ��� �浹(�ߺ� ���� ��)�ȴٸ� �ּ� ó���Ͻʽÿ�.
typedef unsigned char byte;
typedef unsigned int word;
typedef unsigned char boolean;

#define HIGH 0x1
#define LOW  0x0

#define INPUT 0x0
#define OUTPUT 0x1
#define INPUT_PULLUP 0x2

#define true 0x1
#define false 0x0
//------------------------------------------------------------//


void pinMode(int, int);
int digitalRead(int);
unsigned long millis(void);

class Stream
{
  private:

  public:
    Stream();
    int available(void);
    byte read(void);
    void write(int);
}

class HardwareSerial : public Stream
{
  private:

  public:
    HardwareSerial();
    void begin(unsigned long);
};
extern HardwareSerial Serial;

#endif

