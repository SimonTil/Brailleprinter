enum pitches {G3 = 196, C4 = 262, F4 = 349, G4 = 392, C5 = 523};
enum duration {longTone = 100, shortTone = 50, waitTone = 10};

void setup(){
    Serial.begin(9600);
}

void loop(){
    Serial.println("Tone klaar voor gebruik");
    tone(7, C5, shortTone);
    wait(waitTone * 1000);
    tone(7, G4, shortTone);
    delay(2000);

    Serial.println("Tone noodstop ingedrukt!");
    tone(7, F4, longTone);
    wait(waitTone * 1000);
    tone(7, F4, longTone);
    delay(2000);

    Serial.println("Tone papier op");
    tone(7, C4, shortTone);
    wait(waitTone * 1000);
    tone(7, C4, shortTone);
    wait(waitTone * 1000);
    tone(7, C4, shortTone);
    delay(2000);

    Serial.println("Tone volgende regel, wacht op papier");
    tone(7, C4, shortTone);
    delay(2000);

    Serial.println("Tone nieuw papier geladen");
    tone(7, G4, shortTone);
    wait(waitTone);
    tone(7, G4, shortTone);
}

void wait(uint64_t time){
    // Wait while checking buttonS:
    delayMicroseconds(time);
}