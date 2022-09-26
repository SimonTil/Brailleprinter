#ifndef TONE_H
#define TONE_H

#include "Tone.h"

enum pitches
{
    noteC3 = 131,
    noteA3 = 220,
    noteB3 = 247,
    noteD4 = 293,
    noteC5 = 523
};

enum tones
{
    boot,
    noPaper,
    newPaperDetected,
    emergencyStop
};

void signal(tones signal);

#endif