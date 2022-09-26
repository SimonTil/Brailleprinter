#include "Arduino.h"
#include "Tone.h"

void signal(tones signal)
{
    switch (signal)
    {
    case boot:
        tone(6, noteC5, 150);
        delay(200);
        tone(6, noteD4, 200);
        break;
    case noPaper:
        tone(6, noteB3, 150);
        delay(200);
        tone(6, noteB3, 150);
        delay(200);
        tone(6, noteA3, 200);
        break;
    case newPaperDetected:
        tone(6, noteA3, 200);
        delay(150);
        tone(6, noteB3, 200);
        break;
    case emergencyStop:
        tone(6, noteC3, 300);
        delay(500);
        tone(6, noteC3, 600);
        break;
    default:
        break;
    }
}