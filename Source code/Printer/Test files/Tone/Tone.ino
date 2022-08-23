void setup(){
    Serial.begin(9600);
    delay(1000);
}

void loop(){
    Serial.println("Klaar voor gebruik");
    tone(6, 523, 150);
    wait(200);
    tone(6, 292, 200);
    delay(2000);

    Serial.println("Noodstop ingedrukt!");
    tone(6, 131, 300);
    wait(500);
    tone(6, 131, 600);
    delay(2000);

    Serial.println("Papier op");
    tone(6, 247, 150);
    wait(200);
    tone(6, 247, 150);
    wait(200);
    tone(6, 220, 200);
    delay(2000);

    Serial.println("Volgende regel & wacht op papier");
    tone(6, 220, 200);
    delay(2000);

    Serial.println("Nieuw papier geladen");
    tone(6, 220, 200);
    wait(150);
    tone(6, 247, 200);
    delay(20000);
}

void wait(uint16_t time){
    // Wait while checking buttonS:
    delay(time);
}
