void setup(){
  Serial.begin(9600);
  pinMode(9, INPUT_PULLUP);
}

void loop(){
  Serial.println((PINB & 2), BIN);
}
