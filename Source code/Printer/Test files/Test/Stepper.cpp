#include "Stepper.h"
#include "Tone.h"

uint8_t limitSwitchPin = 3;

/* Initialize stepper */
Stepper::Stepper(uint8_t stepPin, uint8_t dirPin)
{
    this->stepPin = stepPin;
    this->dirPin = dirPin;
}

Stepper::Stepper(uint8_t stepPin)
{
    this->stepPin = stepPin;
    this->enPin = 10;
}

/* functions */
void Stepper::takeStep(uint16_t numSteps, uint16_t speed)
{
    for (uint16_t i = 0; i < numSteps; i++)
    {
        digitalWrite(this->stepPin, HIGH);
        wait(speed);
        digitalWrite(this->stepPin, LOW);
        wait(speed);
    }
}

void Stepper::home()
{
    // Return if already homed:
    if (this->homed)
        return;

    // Move toward limit switch:
    if (!digitalRead(limitSwitchPin))
    {
        // Set direction negative:
        digitalWrite(this->dirPin, HIGH);

        // Take steps until limit switch is triggered:
        while (digitalRead(limitSwitchPin))
            this->takeStep(1, 50);
    }

    // Set direction to positive:
    digitalWrite(this->dirPin, LOW);

    // Move away from limit switch:
    this->takeStep(500, 70);

    // Set direction to negative:
    digitalWrite(this->dirPin, HIGH);

    // Move slowly to limit switch:
    while (digitalRead(limitSwitchPin))
        this->takeStep(1, 150);

    // Set direction to positive:
    digitalWrite(this->dirPin, LOW);

    // Move to first character position (offset):
    this->takeStep(10, 150);

    // Set to homed:
    this->homed = true;
}

void Stepper::nextPaper()
{
    tone(6, noteB3, 150);
        wait(200000);
        tone(6, noteB3, 150);
        wait(200000);
        tone(6, noteA3, 200);
    // Turn on stepper motor:
    this->empower(true);

    // Feed until paper sensor is triggered:
    while (!digitalRead(9))
        this->takeStep(1, 100);

        tone(6, noteA3, 200);
        wait(150000);
        tone(6, noteB3, 200);

    // Feed in:
    this->takeStep(4340, 100);

    // Turn off stepper motor:
    this->empower(false);
}

void Stepper::empower(bool turnOn){
    digitalWrite(this->enPin, !turnOn);
    wait(200000);
}