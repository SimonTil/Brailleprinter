#include "Pins.h"
#include "Stepper.h"
#include "Tone.h"

enum states
{
	transmitting,
	printing
} activeState = transmitting;
uint8_t buffer[32];

BraillePin braillePins[] = {
	BraillePin(3, 90, 100),	 // upper left
	BraillePin(5, 90, 100),	 // middle left
	BraillePin(6, 90, 100),	 // lower left
	BraillePin(9, 100, 90),	 // upper right
	BraillePin(10, 100, 90), // middle right
	BraillePin(11, 100, 90)	 // lower right
};

Stepper stepperX(4, 5);
Stepper stepperY(2);

void printChar(uint8_t character)
{
	// Push up concerning pins:
	for (uint8_t i = 0; i < 6; i++)
	{
		if (character >> i & 1)
		{
			braillePins[i].pinUp();
		}
	}
	wait(500000);

	// Release all pins:
	for (BraillePin bp : braillePins)
		bp.pinDown();
	wait(200000);
}

void wait(uint64_t delayTime)
{
	// Set starting time:
	uint64_t startTime = micros();

	while (startTime + delayTime > micros())
	{
		// If button S triggered:
		if (!digitalRead(8))
		{
			// Send "Stop":
			Serial.write(0x32);

			// Turn off power from stepper:
			stepperY.empower(false);

			signal(emergencyStop);

			// Stop everything:
			while (true)
				;
		}
	}
}

void setup()
{
	// Initialize pins:
	for (BraillePin braillePin : braillePins)
		braillePin.initialize();

	pinMode(2, OUTPUT);		  // feed step
	pinMode(3, INPUT_PULLUP); // limit switch
	pinMode(4, OUTPUT);		  // carr step
	pinMode(5, OUTPUT);		  // carr dir
	pinMode(6, OUTPUT);		  // speaker
	pinMode(7, INPUT_PULLUP); // button F
	pinMode(8, INPUT_PULLUP); // button S
	pinMode(9, INPUT_PULLUP); // paper sensor

	// Initialize serial:
	Serial.begin(9600);
	while (!Serial)
		;

	signal(boot);
}

void loop()
{
	if (activeState == transmitting)
	{
		Serial.write(0x31);
		for (uint8_t i = 0; i < 32; i++)
		{
			while (!Serial.available())
			{
				wait(50000);
			}

			buffer[i] = Serial.read();

			if (buffer[i] >= 0b01000000)
			{
				activeState = printing;
				break;
			}
		}
	}
	else // activeState == printing
	{
		stepperX.home();

		if (!digitalRead(9)) // paper sensor
			stepperY.nextPaper();

		for (uint8_t i : buffer)
		{
			if (buffer[i] < 0b01000000)
			{
				if (buffer[i] != 0)
					printChar(buffer[i]);

				if (buffer[i + 1] < 0b01000000)
				{
					stepperX.homed = false;
					stepperX.takeStep(480, 150);
				}
			}
			else
			{
				break;
			}
		}

		stepperX.home();

		stepperY.empower(true);
		stepperY.takeStep(1300, 100);
		stepperY.empower(false);

		memset(buffer, 0, sizeof(buffer));

		activeState = transmitting;
	}
}