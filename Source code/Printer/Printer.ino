// General setup:
#include <Arduino.h>
#include <Wire.h>
#include <Adafruit_PWMServoDriver.h>

enum states {
    transmitting, printing
} activeState = transmitting;
uint8_t buffer[32];

// Setup for servos:
Adafruit_PWMServoDriver servos = Adafruit_PWMServoDriver();
uint16_t servosIdle[6] = { 1560, 1510, 1530, 1500, 1480, 1490 };
uint16_t servosPressed[6] = { 1490, 1400, 1430, 1610, 1590, 1600 };

// Setup for buttons:
#define BUTTON_S_PIN     8
#define BUTTON_F_PIN     7
#define PAPER_SENSOR_PIN 9
#define LIMIT_SWITCH_PIN 3

/* MAIN SETUP */
void setup(){
    Serial.begin(9600);
    
    // Initialize buttons:
    pinMode(BUTTON_S_PIN, INPUT_PULLUP);
    pinMode(BUTTON_F_PIN, INPUT_PULLUP);
    pinMode(PAPER_SENSOR_PIN, INPUT_PULLUP);
    pinMode(LIMIT_SWITCH_PIN, INPUT_PULLUP);

    // Initialize servos:
    servos.begin();
    servos.setOscillatorFrequency(27000000);
    servos.setPWMFreq(50);
    printChar(0);

    // Initialize steppers:
    pinMode(5, OUTPUT); // carriage direction
    pinMode(4, OUTPUT); // carriage step
    pinMode(2, OUTPUT); // feeder step
    home();
}

/* MAIN LOOP */
void loop(){
    if(activeState == transmitting){
        for(int i = 0; i < 32; i++){
            // Wait for computer to receive data, while checking button "F":
            while(!Serial.available()){
                // Next line in case button F is pressed:
                if (!digitalRead(BUTTON_F_PIN)) nextY();

                // Take a breathe to prevent overheating in idle state:
                wait(50);
            }

            // Read single char:
            buffer[i] = Serial.read();

            // In case of new line (0b01000000) or end of file (0b01000001):
            if(buffer[i] >= 0b01000000){
                // Flush serial:
                Serial.flush();

                // Change state to "printing":
                activeState = printing;

                // Break out of for-loop (mandatory):
                break;
            }
        }
    }else{ // activeState == printing
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
        home();

        // Next line:
        nextY();

        // Empty the buffer:
        memset(buffer, 0, sizeof(buffer));

        // Send ready to receive new line:
        activeState = transmitting;
        Serial.write("1");
    }
}

void waitMicroseconds(uint64_t delayTime){
    // Starting time:
    uint64_t startTime = micros();

    // Wait while reading button "S":
    while (startTime + delayTime > micros())
    {
        if(!digitalRead(BUTTON_S_PIN))
        {
            // Send "printer halted":
            Serial.write("2");
            
            // Speaker: "Terminated":
            //tone(SPEAKER_PIN, noteC3, 300);
            //wait(500);
            //tone(SPEAKER_PIN, noteC3, 600);

            // Stop everything until hard reset:
            while(true){}
        }
    }
}
void wait(uint64_t delayTime)
{
    // Starting time:
    uint64_t startTime = millis();

    // Wait while reading button "S":
    while (startTime + delayTime > millis())
    {
        if(!digitalRead(BUTTON_S_PIN))
        {
            // Send "printer halted":
            Serial.write("2");
            
            // Speaker: "Terminated":
            //tone(SPEAKER_PIN, noteC3, 300);
            //wait(500);
            //tone(SPEAKER_PIN, noteC3, 600);

            // Stop everything until hard reset:
            while(true){}
        }
    }
}

/* MOVEMENT FUNCTIONS */
void nextX(){
    // Set direction to positive:
    digitalWrite(5, LOW);

    // Take 480 steps:
    for(uint16_t i = 0; i < 480; i++)
    {
        PORTD = PORTD | B00010000;
        waitMicroseconds(70);
        PORTD = PORTD & B11101111;
        waitMicroseconds(70);
    }
}
void nextY(){
    // Go to next line
    for (uint16_t i = 0; i < 1273; i++) {
        PORTD = PORTD | B00000100;
        waitMicroseconds(100);
        PORTD = PORTD & B11111011;
        waitMicroseconds(100);
    }
}
void nextPaper(){
    // When no paper detected on next line:
    if (digitalRead(9)){
        // Speaker: "Enter new paper":
        //tone(SPEAKER_PIN, noteB3, 150);
        //wait(200);
        //tone(SPEAKER_PIN, noteB3, 150);
        //wait(200);
        //tone(SPEAKER_PIN, noteA3, 200);

        // Outfeed (optional) previous paper:
        for (uint16_t i = 0; i < 750; i++) {
            PORTD = PORTD | B00000100;
            waitMicroseconds(100);
            PORTD = PORTD & B11111011;
            waitMicroseconds(100);
        }

        // Feed until paper sensor is triggered:
        while (digitalRead(9)) {
            PORTD = PORTD | B00000100;
            waitMicroseconds(100);
            PORTD = PORTD & B11111011;
            waitMicroseconds(100);
        }

        // Speaker: "New paper detected":
        //tone(SPEAKER_PIN, noteA3, 200);
        //wait(150);
        //tone(SPEAKER_PIN, noteB3, 200);

        // Feed in:
        for (uint16_t i = 0; i < 2000; i++) {
            PORTD = PORTD | B00000100;
            waitMicroseconds(100);
            PORTD = PORTD & B11111011;
            waitMicroseconds(100);
        }
    }
}
void home()
{
    // Set direction to HIGH:
    digitalWrite(5, HIGH);

    // Take steps until endstop is triggered:
    while (digitalRead(LIMIT_SWITCH_PIN))
    {
        PORTD = PORTD | B00010000;
        waitMicroseconds(50);
        PORTD = PORTD & B11101111;
        waitMicroseconds(50);
    }

    // Set direction to LOW:
    digitalWrite(5, LOW);

    // Move to first character position (offset):
    for(uint16_t i = 0; i < 110; i++)
    {
        PORTD = PORTD | B00010000;
        waitMicroseconds(70);
        PORTD = PORTD & B11101111;
        waitMicroseconds(70);
    }
}

/* PRINTHEAD FUNCTIONS */
void printChar(uint8_t character){
    // Wake up servo driver (for power distribution):
    servos.wakeup();

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
    wait(200);

    if(character != 0){
        // Hold pins:
        wait(300);

        // Release all servos:
        for (uint8_t i = 0; i < 6; i++) {
            servos.writeMicroseconds(i, servosIdle[i]);
        }

        // Wait for pins to go down:
        wait(200);
    }
    // Turn off servo driver:
    servos.sleep();
}