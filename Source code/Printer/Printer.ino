// General setup:
#include <Arduino.h>
enum states {transmitting, printing} activeState = transmitting;
bool printInProcess = false;
uint8_t buffer[32];
uint8_t lastCharOfLine = 0;

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

// Setup for servos:
#include <Wire.h>
#include <Adafruit_PWMServoDriver.h>
Adafruit_PWMServoDriver servos = Adafruit_PWMServoDriver();
uint16_t servosIdle[6] = { 1560, 1510, 1530, 1500, 1480, 1490 };
uint16_t servosPressed[6] = { 1490, 1400, 1430, 1610, 1590, 1600 };

// Setup for steppers:
#define CARR_DIR_PIN     5
#define CARR_STEP_PIN    4
#define FEED_STEP_PIN    2
struct speed {
    uint64_t x = 500;
    uint64_t y = 500;
} speed;

void setup()
{
    // General:
    Serial.begin(9600);
    
    // Initialize servos:
    // servos.begin();
    // servos.setOscillatorFrequency(27000000);
    // servos.setPWMFreq(50);
    // servosUp();
    // delay(200);

    // // Initialize steppers:
    // pinMode(CARR_DIR_PIN, OUTPUT);
    // pinMode(CARR_STEP_PIN, OUTPUT);
    // pinMode(FEED_STEP_PIN, OUTPUT);
    // home();

    // // Initialize buttons:
    // pinMode(BUTTON_S_PIN, INPUT_PULLUP);
    // pinMode(BUTTON_F_PIN, INPUT_PULLUP);
    // pinMode(PAPER_SENSOR_PIN, INPUT_PULLUP);
    // pinMode(LIMIT_SWITCH_PIN, INPUT_PULLUP);
    
    // // Initialize speaker:
    // pinMode(SPEAKER_PIN, OUTPUT);
    // tone(SPEAKER_PIN, noteC5, 150);
    // wait(200);
    // tone(SPEAKER_PIN, noteD4, 200);
    // wait(200);
}

void loop()
{
    /* Meaning of state transmitting:
     * In this state, data can be send from computer to brailleprinter.
     * When there is no more data to transmit, it turns into idle state.
     * Paper can be feed out by pressing button F.
     */
    if(activeState == transmitting)
    {
        // Send "ready to receive new set":
        Serial.write("1");
        wait(10);

        // Receive up to 31 chars (optional 0b01000000/0b01000001 at end of string):
        for(int i = 0; i < 32; i++)
        {
            // Wait until serial is available, meanwhile checking button F:
            while(!Serial.available())
            {
                // Check button F:
                // if(!digitalRead(BUTTON_F_PIN))
                // {
                //     // Outfeed paper (hold button "F" on printer):
                //     nextY();
                // }

                // Wait 10 ms:
                wait(10);
            }

            // Read single char when available:
            if(Serial.available())
            {
                // Set printer to be busy with a printjob
                //printInProcess = true;

                // Read single char:
                buffer[i] = Serial.read();

                // In case of new line (0b01000000) or end of file (0b01000001):
                if(buffer[i] >= 0b01000000)
                {
                    // Set end of line:
                    //lastCharOfLine = i;

                    // Change state to "printing":
                    activeState = printing;

                    // Flush serial:
                    Serial.flush();

                    // Break out of loop:
                    break;
                }
            }
        }
    }
    /* Meaning of state printing:
     * In this state, no data can be send from computer to brailleprinter.
     * Print char by char.
     * - In case of 0b01000000, go to next line;
     * - In case of 0b01000001, go to next line and go to state transmitting.
     */
    else
    {
        // Iterate through string
        for(int i = 0; i < 32; i++)
        {
            // Print single char if not end of line, or end of file:
            if(buffer[i] < 0b0100000)
            {
                // Print single char, except spaces:
                if(buffer[i] != 0){} //printChar(buffer[i]);

                // Move to next position:
                if(buffer[i + 1] < 0b01000000 && i != lastCharOfLine)
                {
                    //nextX();
                }
                else
                {
                    break;
                }
            }
            else if(buffer[i] == 0b01000001) printInProcess = false;
        }

        // Move to position 0:
        //home();
            
        // Next line:
        //nextY();

        // Empty the array:
        //memset(buffer, 0, sizeof(buffer));

        // Receive new line
        activeState = transmitting;
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
            tone(SPEAKER_PIN, noteC3, 300);
            wait(500);
            tone(SPEAKER_PIN, noteC3, 600);

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
            tone(SPEAKER_PIN, noteC3, 300);
            wait(500);
            tone(SPEAKER_PIN, noteC3, 600);

            // Stop everything until hard reset:
            while(true){}
        }
    }
}

void nextX()
{
    uint16_t numOfSteps = 520;

    // Set direction to HIGH:
    digitalWrite(CARR_DIR_PIN, HIGH);

    // Take <numOfSteps> steps:
    for(uint16_t i = 0; i < numOfSteps; i++)
    {
        PORTD = PORTD | B00010000;
        waitMicroseconds(speed.x / 2);
        PORTD = PORTD & B00010000;
        waitMicroseconds(speed.x / 2);
    }
}
void home()
{
    // Set direction to LOW:
    digitalWrite(CARR_DIR_PIN, LOW);

    // Take steps until endstop is triggered:
    while (digitalRead(LIMIT_SWITCH_PIN))
    {
        PORTD = PORTD | B00010000;
        waitMicroseconds(speed.x / 2);
        PORTD = PORTD & B00010000;
        waitMicroseconds(speed.x / 2);
    }

    // Set direction to HIGH:
    digitalWrite(CARR_DIR_PIN, HIGH);

    // Move to first character position (offset):
    for(uint8_t i = 0; i < 20; i++)
    {
        PORTD = PORTD | B00010000;
        waitMicroseconds(speed.x / 2);
        PORTD = PORTD & B00010000;
        waitMicroseconds(speed.x / 2);
    }
}
void nextY()
{
    uint16_t numOfSteps = 1500, infeedSteps = 1000, outfeedSteps = 500;

    // Speaker: "Next line":
    tone(SPEAKER_PIN, noteA3, 200);

    // Go to next line:
    for (uint16_t i = 0; i < numOfSteps; i++)
    {
        PORTD = PORTD | B00000100;
        waitMicroseconds(speed.y / 2);
        PORTD = PORTD & B00000100;
        waitMicroseconds(speed.y / 2);
    }
    
    // When no paper detected on next line:
    if(!digitalRead(9))
    {
        // Speaker: "Enter new paper":
        tone(SPEAKER_PIN, noteB3, 150);
        wait(200);
        tone(SPEAKER_PIN, noteB3, 150);
        wait(200);
        tone(SPEAKER_PIN, noteA3, 200);

        // Outfeed paper:
        for(int i = 0; i < outfeedSteps; i++){
            PORTD = PORTD | B00000100;
            waitMicroseconds(speed.y / 2);
            PORTD = PORTD & B00000100;
            waitMicroseconds(speed.y / 2);
        }

        // If still in print process, wait for new paper:
        if(printInProcess)
        {
            // Feed until paper sensor is triggered:
            while(!digitalRead(9))
            {
                PORTD = PORTD | B00000100;
                waitMicroseconds(speed.y / 2);
                PORTD = PORTD & B00000100;
                waitMicroseconds(speed.y / 2);
            }

            // Speaker: "New paper detected":
            tone(SPEAKER_PIN, noteA3, 200);
            wait(150);
            tone(SPEAKER_PIN, noteB3, 200);

            // Feed in:
            for (int i = 0; i < infeedSteps; i++){
                PORTD = PORTD | B00000100;
                waitMicroseconds(speed.y / 2);
                PORTD = PORTD & B00000100;
                waitMicroseconds(speed.y / 2);
            }
        }
    }
}

void printChar(uint8_t character)
{
    // Press down relevant servos:
    for(uint8_t i = 0; i < 6; i++)
    {
        if(character >> i & 0b1)
        {
            // Press down single servo and wait for next one:
            servos.writeMicroseconds(i, servosPressed[i]);
            delay(1);
        }
    }

    // Wait until all servos are down:
    wait(200);

    // Release servos:
    servosUp();
}
void servosUp()
{
    // Release all servos:
    for(uint8_t i = 0; i < 6; i++)
    {
        servos.writeMicroseconds(i, servosIdle[i]);
        delay(1);
    }

    // Wait until servos are released:
    wait(400);
}