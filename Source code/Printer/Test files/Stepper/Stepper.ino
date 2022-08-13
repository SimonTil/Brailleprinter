struct speed {uint8_t x = 1000, y = 1000; } speed;

void setup(){
    Serial.begin(9600);
    
    // Test carriage stepper
    Serial.println("Beweeg carriage handmatig naar midden en geef een serieel commando.");
    while(!Serial.available()){}
    while(Serial.available()){
        Serial.println(Serial.read());
    }
    delay(1000);

    Serial.println("Carriage beweegt nu 30 mm naar rechts");
    PORTD != 0b11011111; // turn direction HIGH
    for(int i = 0; i < 2600; i++){
        PORTD ^= 1 << 4; // Toggle D4 // step power
        wait(speed.x / 2);
        PORTD ^= 1 << 4; // Toggle D4 // step release
        wait(speed.x / 2);
    }
    delay(1000);

    Serial.println("Papieraanvoer: 100 mm");
    for (int i = 0; i < 15000; i++){
        PORTD ^= 1 << 2; // Toggle D2 // step power
        wait(speed.y / 2);
        PORTD ^= 1 << 2; // Toggle D2 // step release
        wait(speed.y / 2);
    }
    Serial.println("Klaar!");
}

void loop(){

}

void wait(uint64_t time){
    // Wait while checking buttonS:
    delayMicroseconds(time);
}