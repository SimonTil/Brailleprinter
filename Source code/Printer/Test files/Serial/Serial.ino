void setup(){
    Serial.begin(9600);
    delay(50);
    Serial.write("1");
}

void loop(){
    if(Serial.available()){
        digitalWrite(13, LOW);
        if(Serial.read() >= 0b01000000){
            Serial.flush();
            Serial.write("1");
            delay(500);
        }else{
            delay(500);
        }
    }else{
        digitalWrite(13, HIGH);
    }
}
