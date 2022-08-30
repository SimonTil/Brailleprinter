// General setup:
#include <Arduino.h>
enum states {transmitting, printing} activeState = transmitting;
uint8_t buffer[32];

// Setup for buttons:
#define BUTTON_S_PIN     7
#define BUTTON_F_PIN     8
#define PAPER_SENSOR_PIN 9
#define LIMIT_SWITCH_PIN 3

// Setup for speaker:
#define SPEAKER_PIN      6
enum notes {noteD4 = 294, noteC5 = 523};

void setup()
{
    // General:
    Serial.begin(9600);
    
    // Initialize pins:
    pinMode(BUTTON_S_PIN, INPUT_PULLUP);
    pinMode(BUTTON_F_PIN, INPUT_PULLUP);
    pinMode(PAPER_SENSOR_PIN, INPUT_PULLUP);
    pinMode(LIMIT_SWITCH_PIN, INPUT_PULLUP);
    pinMode(SPEAKER_PIN, OUTPUT);

    // Initialize steppers:
    home();

    // Speaker "ready":
    tone(SPEAKER_PIN, noteC5, 150); // pin, frequency, duration
    wait(200);
    tone(SPEAKER_PIN, noteD4, 200);
    wait(200);
}

void loop()
{
    // Transmitting state:
    if(activeState == transmitting)
    {
        // Send "ready to receive new set":
        Serial.println("1");
        wait(10);

        // Receive up to 31 chars:
        for(int i = 0; i < 32; i++)
        {
            // Wait until serial is available, meanwhile checking button F:
            while(!Serial.available())
            {
                // Feed paper if button "F" is pressed:
                if(!digitalRead(BUTTON_F_PIN)) nextY();
            }

            // Read single char when available:
            if(Serial.available())
            {
                // Read single char:
                buffer[i] = Serial.read();

                if(buffer[i] >= 0b01000000)
                {
                    // Change state to "printing":
                    activeState = printing;

                    // Flush serial:
                    while(Serial.available()) Serial.read();

                    // Break out of loop:
                    break;
                }
            }
        }
    }
    // Printing state:
    else
    {
        // Print char by char:
        for(int i = 0; i < 32; i++)
        {
            // Print single char, except spaces:
            if(buffer[i] < 0b01000000)
            {
                // Print single char:
                if(buffer[i] != 0) printChar(buffer[i]);

                // Move to next position if next char is not enter:
                if(buffer[i + 1] < 0b01000000) nextX();
            }
        }

        // Go to position 0:
        home();

        // Move to next line
        nextY();

        // Empty buffer:
        memset(buffer, 0, sizeof(buffer));

        // Change state to "transmitting":
        activeState = transmitting;
    }
}