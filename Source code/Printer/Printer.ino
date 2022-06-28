#include <Arduino.h>
#include <Wire.h>
#include <Adafruit_PWMServoDriver.h>

// State:
enum states {transmitting, printing} activeState = transmitting;

// Time management:
uint64_t servoDownDelay = 20000, servoUpDelay = 20000;

// Printing:
Adafruit_PWMServoDriver servos = Adafruit_PWMServoDriver();
uint16_t servosIdle[6] = {150, 150, 150, 180, 180, 180};
uint16_t servosPressed[6] = {180, 180, 180, 150, 150, 150};
uint8_t line[32]; // 31 positions, including line feed

// Steppers
struct position { uint8_t x, y = 27; bool homed = false; } pos;
struct speed {uint8_t x = 100, y = 100; } speed;

// Speaker:
enum pitches {G3 = 196, C4 = 262, F4 = 349, G4 = 392, C5 = 523};
enum duration {longTone = 100, shortTone = 50, waitTone = 10};

void setup(){
    // Initialize pins:
    pinMode(8, INPUT_PULLUP); // buttonS
    pinMode(A1, INPUT_PULLUP); // paperSensor
    pinMode(10, INPUT_PULLUP); // buttonF

    // Initialize servos:
    servos.begin();
    servos.setOscillatorFrequency(27000000);
    servos.setPWMFreq(50);
    delay(10);
    servosUp();
    wait(20000); // Wait 20 millisecons

    // Initialize steppers:
    home();
    
    // Tone ready:
    tone(7, C5, shortTone);
    wait(waitTone * 1000);
    tone(7, G4, shortTone);

    nextPaper();
}

void loop(){
    // Read "stop"-button:
    if (activeState == transmitting){
        // Read 32 characters, or until 0b01_000000 or 0b01_000001 is passed:
        Serial.println(1); // Send "ready"
        wait(10000); // wait 10 milliseconds

        for (int i = 0; i < 32; i++){
            // Wait until serial is available:
            while (!Serial.available() > 0){
                // No serial available, check if buttonF is pressed:
                if ((PINB & 0b00000100) == 0) nextY();
                // Wait 1 millisecond while checking if buttonS is pressed:
                wait(1000);
            }

            // Read next char:
            line[i] = Serial.read();

            // Break in case of \n or end-of-file
            if (line[i] >= 0b01000000){
                activeState = printing;
                // Flush serial:
                while (Serial.available() > 0){ Serial.read(); }
                break;
            }
        }
    } else { // activeState == printing
        for (int i = 0; i < 32; i++){
            // Print char (if not space)
            if (line[i] < 0b01000000){
                if (line[i] != 0) printChar(line[i]);
                if (i < 31) nextX();
            } else if (line[i] >= 0b01000000){
                home();
                nextY();
            }
        }

        // Empty array and receive new line
        memset(line, 0, sizeof(line));
        activeState = transmitting;
    }
}

void wait(uint64_t time){
    uint64_t currentMicros = micros();

    // Wait while checking buttonS:
    while (currentMicros + time < micros()){
        if ((PINB & 0b00000001) == 0){
            tone(7, F4, longTone);
            wait(waitTone * 1000);
            tone(7, F4, longTone);
            while(true){} // Stop everything
        }
    }
}

void printChar(uint8_t character){
    // Press down servos:
    for (int i = 0; i < 6; i++){
        if (character >> i & 0b1){
            servos.setPWM(i, 0, servosPressed[i]);
        }
    }
    wait(servoDownDelay);
    servosUp();
    wait(servoUpDelay);
}

void servosUp(){
    // Release servos:
    for (int i = 0; i < 6; i++){
        servos.setPWM(i, 0, servosIdle[i]);
    }
}

void home(){
    uint16_t homeOffset = 20;
    PORTC &= 0b11110111; // turn direction LOW

    // Move until limit switch is triggered:
    while (PINC & 0b10 == 1){ // Read A1
        PORTC ^= 1 << 2; // Toggle A2
        wait(speed.x / 2);
        PORTC ^= 1 << 2; // Toggle A2
        wait(speed.x / 2);
    }

    // Move offset:
    PORTC != 0b00001000; // turn direction HIGH
    for (int i = 0; i < homeOffset; i++){
        PORTC ^= 1 << 2; // Toggle A2
        wait(speed.x / 2);
        PORTC ^= 1 << 2; // Toggle A2
        wait(speed.x / 2);
    }
    pos.x = 0;
}

void nextX(){
    uint16_t numOfSteps = 520;

    // Go to next character:
    for (int i = 0; i < numOfSteps; i++){
        PORTC ^= 1 << 2; // Toggle A2
        wait(speed.x / 2);
        PORTC ^= 1 << 2; // Toggle A2
        wait(speed.x / 2);
    }
    pos.x++;
}

void nextY(){
    uint16_t numOfSteps = 1500;

    if (pos.y == 27){
        tone(7, C4, shortTone);
        wait(waitTone * 1000);
        tone(7, C4, shortTone);
        wait(waitTone * 1000);
        tone(7, C4, shortTone);
        nextPaper();
    } else {
        tone(7, C4, shortTone);

        // Go to next line:
        for (int i = 0; i < numOfSteps; i++){
            PORTC ^= 1 << 2; // Toggle A2
            wait(speed.y / 2);
            PORTC ^= 1 << 2; // Toggle A2
            wait(speed.y / 2);
        }
        pos.y++;
    }
}

void nextPaper(){
    uint32_t startCounter = millis();
    uint32_t iterator = 0;
    uint16_t outfeedSteps = 3500, infeedSteps = 1000;

    // Feed out:
    for (int i = 0; i < outfeedSteps; i++){
        PORTC ^= 1 << 2; // Toggle A2
        wait(speed.y / 2);
        PORTC ^= 1 << 2; // Toggle A2
        wait(speed.y / 2);
    }

    // Wait for paper:
    tone(7, C4, shortTone);
    while (PINC & 0b10 == 1){ // Wait until A1 is LOW
        PORTC ^= 1 << 2; // Toggle A2
        wait(speed.y / 2);
        PORTC ^= 1 << 2; // Toggle A2
        wait(speed.y / 2);
        // Beep every second until paper is present
        if (startCounter + iterator > millis()){
            iterator += 1000;
            tone(7, C4, shortTone);
        }
    }

    tone(7, G4, shortTone);
    wait(waitTone);
    tone(7, G4, shortTone);

    // Feed in:
    for (int i = 0; i < infeedSteps; i++){
        PORTC ^= 1 << 2; // Toggle A2
        wait(speed.y / 2);
        PORTC ^= 1 << 2; // Toggle A2
        wait(speed.y / 2);
    }
    pos.y = 0;
}