// Prints 4 values: button 1, button 2, paper sensor, and enstop

void setup(){
    Serial.begin(9600);
    pinMode(7, INPUT_PULLUP); // button 1
    pinMode(8, INPUT_PULLUP); // button 2
    pinMode(9, INPUT_PULLUP); // paper sensor
    pinMode(3, INPUT_PULLUP); // endstop
}

void loop(){
    Serial.print(digitalRead(7));
    Serial.print(", ");
    Serial.print(digitalRead(8));
    Serial.print(", ");
    Serial.print(digitalRead(9));
    Serial.print(", ");
    Serial.println(digitalRead(3));
    delay(10);
}
