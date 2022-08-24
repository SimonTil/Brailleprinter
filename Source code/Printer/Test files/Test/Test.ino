// General setup:
#include <Arduino.h>
enum states {transmitting, printing} activeState = transmitting;

// Setup for buttons:
#define BUTTON_S_PIN     7
#define BUTTON_F_PIN     8
#define PAPER_SENSOR_PIN 9
#define LIMIT_SWITCH_PIN 3

// Setup for speaker:
#define SPEAKER_PIN      6

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

    // Speaker "ready":
    tone(SPEAKER_PIN, 523, 150); // pin, frequency, duration
    delay(200);
    tone(SPEAKER_PIN, 292, 200);
    delay(200);
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
        Serial.println("1");
        delay(10);

        // Receive up to 31 chars:
        for (int i = 0; i < 32; i++)
        {
            // Wait until serial is available, meanwhile checking button F:
            while (!Serial.available())
            {
                // Check button F:
                if(!digitalRead(BUTTON_F_PIN))
                {
                    // Outfeed paper:
                }
            }

            // Read single char when available:
            if (Serial.available())
            {
                // Read single char:
                line[i] = Serial.read();

                if (line[i] >= 0b01000000)
                {
                    // Change state to "printing":
                    activeState = printing;

                    // Flush serial:
                    while (Serial.available()) Serial.read();

                    // Break out of loop:
                    break;
                }
            }
        }
    }
    else
    {

    }
}