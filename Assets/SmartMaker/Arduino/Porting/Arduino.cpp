/*
  Arduino.cpp - Arduino ������ ���� �ҽ�
*/

#include "Arduino.h"


void pinMode(int pin, int mode)
{
	/* ���� ��
	   ����
	   : �� �Լ��� GPIO���� ��� ���¸� �����ϱ� ���� ���ȴ�.

	   pin
	   : Arduino �� ��ȣ (���� ȯ�濡 ������ pin ���� �ʿ�)
	   
	   mode
	   : OUTPUT: ���� pin�� OUTPUT���·� �����.
	   : INPUT: ���� pin�� INPUT���·� �����.
	   : INPUT_PULLUP: ���� pin�� ���� PULL UP��� INPUT���·� �����.
	*/
}

int digitalRead(int pin)
{
	/* ���� ��
	   ����
	   : �� �Լ��� ��� ���� pinMode�� ���� INPUT Ȥ�� INPUT_PULLUP�� �����Ǿ�� �Ѵ�.

	   pin
	   : Arduino �� ��ȣ (���� ȯ�濡 ������ pin ���� �ʿ�)
	   
	   return
	   : ���� �� ���¸� 0 Ȥ�� 1�� ��ȯ
	*/
}

unsigned long millis()
{
	/* ���� ��
	   ���� ����
	   : �� �Լ��� �ý����� ���� ���õ� ���ĺ��� msec������ �ð��� ��ȯ�Ѵ�.
	   : 32bit�̹Ƿ� �� 20�ð��� ������ �� �ִ�.
	   : �ַ� ���� �ð��� �ƴ� ��� �ð�(�ð� ����)�� �����ϴµ� ���ȴ�.

	   return
	   : ���õ� ���ĺ��� msec������ �ð�
	*/
}


Stream::Stream()
{
	/* ���� ��
	   ����
	   : ���۸� �ʱ�ȭ��Ų��.
	*/
}

int Stream::available()
{
	/* ���� ��
	   ����
	   : �� Ŭ������ ���� �����͸� Ring ���ۿ� �����Ѵ�.
	   
	   return
	   : ���� ���ۿ� �ִ� ������ �� (byte)
	*/
}

int Stream::read()
{
	/* ���� ��
	   ����
	   : ���ۿ��� �����͸� ������, ���ۿ��� �����Ѵ�.
	   : �� �Լ��� ����ϱ� ���� available�� ���� ������ ���� ������ Ȯ���ؾ� �Ѵ�.

	   return
	   : ���� ���� ���� �տ� �ִ� ������ ��ȯ
	*/
}

void Stream::write(int data)
{
	/* ���� ��
	   ����
	   : UART�� ���� �����͸� ��������.
	   : �񵿱� ����� �������� �����Ƿ� ������ �������Ⱑ �Ϸ�ɶ����� Block��Ų��.
	   : Block��⸦ �ּ�ȭ�ϱ� ���� �۽� ���۸� ����� �� ������, ���� ���۰� ���� Block���Ѿ� �Ѵ�.

	   data
	   : ������ ������ (byte)
	*/
}

HardwareSerial::HardwareSerial() : Stream()
{
	/* ���� ��
	   ����
	   : UART�� �ʱ�ȭ��Ų��.
	*/
}

void HardwareSerial::begin(unsigned long bps)
{
	/* ���� ��
	   ����
	   : UART�� ��� �غ��Ų��.

	   bps
	   : baudrate speed
	*/
}

HardwareSerial Serial;
