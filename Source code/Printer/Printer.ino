// General setup:
#include <Arduino.h>
#include <Wire.h>
#include <Adafruit_PWMServoDriver.h>

enum states {
    transmitting, printing
} activeState = transmitting;
uint8_t buffer[32];
bool homed = false;

// Setup for servos:
Adafruit_PWMServoDriver servos = Adafruit_PWMServoDriver();
uint16_t servosIdle[6] = { 1400, 1600, 890, 1550, 1480, 1480 };
uint16_t servosPressed[6] = { 1240, 1410, 750, 1700, 1620, 1610 };

// Setup for buttons:
#define BUTTON_S_PIN     8
#define BUTTON_F_PIN     7
#define PAPER_SENSOR_PIN 9
#define LIMIT_SWITCH_PIN 3

// Setup for speaker:
#define SPEAKER_PIN      6
enum pitches {
    noteC3 = 131,
    noteA3 = 220,
    noteB3 = 247,
    noteD4 = 293,
    noteC5 = 523
};

/* MAIN SETUP */
void setup(){
    Serial.begin(9600);
    
    // Initialize servos:
    servos.begin();
    servos.setOscillatorFrequency(27000000);
    servos.setPWMFreq(50);
    for(int i = 0; i < 6; i++){
        servos.writeMicroseconds(i, servosIdle[i]);
    }

    // Initialize buttons:
    pinMode(BUTTON_S_PIN, INPUT_PULLUP);
    pinMode(BUTTON_F_PIN, INPUT_PULLUP);
    pinMode(PAPER_SENSOR_PIN, INPUT_PULLUP);
    pinMode(LIMIT_SWITCH_PIN, INPUT_PULLUP);

    // Initialize steppers:
    pinMode(5, OUTPUT); // carriage direction
    pinMode(4, OUTPUT); // carriage step
    pinMode(2, OUTPUT); // feeder step
    digitalWrite(10, HIGH); // turn off feeder

    // Initialize speaker:
    pinMode(SPEAKER_PIN, OUTPUT);
    tone(SPEAKER_PIN, noteC5, 150);
    wait(200000);
    tone(SPEAKER_PIN, noteD4, 200);
    wait(200000);
}

/* MAIN LOOP */
void loop(){
    if(activeState == transmitting){
        // Send "Ready to receive line":
        Serial.write("1");
        // Read up to 32 characters from serial:
        for(int i = 0; i < 32; i++){
            // Wait for computer to receive data, while checking button "F":
            while(!Serial.available()){
                // Next line in case button F is pressed:
                if (!digitalRead(BUTTON_F_PIN)) nextY();

                // Take a breathe to prevent overheating in idle state:
                wait(50000);
            }

            // Read single char:
            buffer[i] = Serial.read();
            delay(1);

            // In case of new line (0b01000000) or end of file (0b01000001):
            if(buffer[i] >= 0b01000000){
                Serial.write(buffer, 32);
                // Change state to "printing":
                activeState = printing;

                // Break out of for-loop (mandatory):
                break;
            }
        }
    }else{ // activeState == printing
        // Home if not yet homed:
        if(!homed) home();
        
        // If no paper detected, feed until new paper inserted:
        nextPaper();

        // Print entire line:
        for(int i = 0; i < 32; i++){
            // Print single char:
            if(buffer[i] < 0b01000000){
                // Print single char, except spaces:
                if(buffer[i] != 0b0) printChar(buffer[i]);
                
                // Move to next position or break out of for-loop:
                if(buffer[i + 1] < 0b01000000){
                    nextX();
                }
            }else{
                // In case of end of file, stop printjob:
                break;
            }
        }

        // Move to position 0:
        if(!homed) home();

        // Next line:
        nextY();

        // Empty the buffer:
        memset(buffer, 0, sizeof(buffer));

        // Send ready to receive new line:
        activeState = transmitting;
    }
}

void wait(uint64_t delayTime){
    // Starting time:
    uint64_t startTime = micros();

    // Wait while reading button "S":
    while (startTime + delayTime > micros())
    {
        if(!digitalRead(BUTTON_S_PIN))
        {
            // Send "printer halted":
            Serial.write("2");

            // Wait until button is released:
            while(!digitalRead(BUTTON_S_PIN));

            // Speaker: "Terminated":
            tone(SPEAKER_PIN, noteC3, 300);
            wait(500000);
            tone(SPEAKER_PIN, noteC3, 600);

            // Stop everything until hard reset:
            while(true){}
        }
    }
}

/* MOVEMENT FUNCTIONS */
void nextX(){
    // Set to not homed:
    homed = false;

    // Set direction to positive:
    digitalWrite(5, LOW);

    // Take 480 steps:
    for(uint16_t i = 0; i < 480; i++)
    {
        PORTD = PORTD | B00010000;
        wait(150);
        PORTD = PORTD & B11101111;
        wait(150);
    }
}
void nextY(){
    // Turn on stepper motor:
    digitalWrite(10, LOW);
    wait(200000);

    // Go to next line
    for (uint16_t i = 0; i < 1300; i++) {
        PORTD = PORTD | B00000100;
        wait(100);
        PORTD = PORTD & B11111011;
        wait(100);
    }

    // Turn off stepper motor
    wait(10000);
    digitalWrite(10, HIGH);
    wait(200000);
}
void nextPaper(){
    // When no paper detected on next line:
    if ((PINB & 2) != 0){ // fast !digitalRead(9)
        // Speaker: "Enter new paper":
        tone(SPEAKER_PIN, noteB3, 150);
        wait(200000);
        tone(SPEAKER_PIN, noteB3, 150);
        wait(200000);
        tone(SPEAKER_PIN, noteA3, 200);

        // Turn on stepper motor:
        digitalWrite(10, LOW);
        wait(200000);

        // Outfeed (optional) previous paper:
        for (uint16_t i = 0; i < 8111; i++) {
            PORTD = PORTD | B00000100;
            wait(100);
            PORTD = PORTD & B11111011;
            wait(100);
        }

        // Feed until paper sensor is triggered:
        while ((PINB & 2) != 0) { // fast !digitalRead(9)
            PORTD = PORTD | B00000100;
            wait(100);
            PORTD = PORTD & B11111011;
            wait(100);
        }

        // Speaker: "New paper detected":
        tone(SPEAKER_PIN, noteA3, 200);
        wait(150000);
        tone(SPEAKER_PIN, noteB3, 200);

        // Feed in:
        for (uint16_t i = 0; i < 4340; i++) {
            PORTD = PORTD | B00000100;
            wait(100);
            PORTD = PORTD & B11111011;
            wait(100);
        }

        // Turn off stepper motor
        digitalWrite(10, HIGH);
        wait(200000);
    }
}
void home()
{
    if(!digitalRead(LIMIT_SWITCH_PIN)){ // if limit switch triggered
        // Set direction to positive:
        digitalWrite(5, LOW);

        // Take steps until endstop is no longer triggered
        while (!digitalRead(LIMIT_SWITCH_PIN)){
            PORTD = PORTD | B00010000;
            wait(50);
            PORTD = PORTD & B11101111;
            wait(50);
        }
    }else{
        // Set direction to negative:
        digitalWrite(5, HIGH);

        // Take steps until endstop is triggered:
        while (digitalRead(LIMIT_SWITCH_PIN)){
            PORTD = PORTD | B00010000;
            wait(50);
            PORTD = PORTD & B11101111;
            wait(50);
        }
    }

    // Set direction to positive:
    digitalWrite(5, LOW);

    // Take a couple of steps:
    for(uint16_t i = 0; i < 500; i++)
    {
        PORTD = PORTD | B00010000;
        wait(70);
        PORTD = PORTD & B11101111;
        wait(70);
    }

    // Set direction to negative:
    digitalWrite(5, HIGH);

    // Home slowly:
    while (digitalRead(LIMIT_SWITCH_PIN)){
        PORTD = PORTD | B00010000;
        wait(200);
        PORTD = PORTD & B11101111;
        wait(200);
    }

    // Set direction to positive:
    digitalWrite(5, LOW);

    // Move to first character position (offset):
    for(uint16_t i = 0; i < 10; i++)
    {
        PORTD = PORTD | B00010000;
        wait(150);
        PORTD = PORTD & B11101111;
        wait(150);
    }

    // Set homed:
    homed = true;
}

/* PRINTHEAD FUNCTIONS */
void printChar(uint8_t character){
    // Wake up servo driver (for power distribution):
    servos.wakeup();
    wait(10000);

    // set all servos:
    for (uint8_t i = 0; i < 6; i++) {
        // Iterate through each pin in 6-digit input:
        if (character >> i & 0b1) {
            // Press down single servo:
            servos.writeMicroseconds(i, servosPressed[i]);
        }else{
            // Keep non-active servo idle:
            servos.writeMicroseconds(i, servosIdle[i]);
        }
    }

    // Wait for pins to go up:
    wait(500000);

    // Release all servos:
    for (uint8_t i = 0; i < 6; i++) {
        servos.writeMicroseconds(i, servosIdle[i]);
    }

    // Wait for pins to go down:
    wait(200000);

    // Turn off servo driver:
    servos.sleep();
    wait(10000);
}