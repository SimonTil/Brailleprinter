#ifndef STEPPER_H
#define STEPPER_H

#include <Arduino.h>
#include "Stepper.h"

void wait(uint64_t delayTime);

class Stepper
{
public:
    Stepper(uint8_t stepPin, uint8_t dirPin);
    Stepper(uint8_t stepPin);
    void home();
    void nextPaper();
    void takeStep(uint16_t numSteps, uint16_t speed);
    void empower(bool turnOn);
    bool homed = false;

private:
    uint8_t stepPin;
    uint8_t dirPin;
    uint8_t enPin;
};

#endif