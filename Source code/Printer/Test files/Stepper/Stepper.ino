int stepCarrPin = 4;
int stepFeedPin = 2;
int dirCarrPin = 5;
int endstopPin = 3;
uint64_t speedDelay = 63;

void setup(){
  pinMode(stepCarrPin, OUTPUT);
  pinMode(stepFeedPin, OUTPUT);
  pinMode(dirCarrPin, OUTPUT);
  pinMode(endstopPin, INPUT_PULLUP);

  // First home the carriage
  PORTD = PORTD | B00100000;
  while((PIND & 0b00001000) != 0){
    PORTD = PORTD | B00010000;
    waitMicroseconds(speedDelay);
    PORTD = PORTD & B11101111;
    waitMicroseconds(speedDelay);
  }
}

void loop(){
  PORTD = PORTD & B11011111;
  for(int i = 0; i < 30; i++){
    delay(300);
    for(int j = 0; j < 480; j++){
    PORTD = PORTD | B00010000;
    waitMicroseconds(speedDelay);
    PORTD = PORTD & B11101111;
    waitMicroseconds(speedDelay);
  }
  }
  delay(300);
  PORTD = PORTD | B00100000;
  while((PIND & 0b00001000) != 0){
    PORTD = PORTD | B00010000;
    waitMicroseconds(speedDelay);
    PORTD = PORTD & B11101111;
    waitMicroseconds(speedDelay);
  }
}

void waitMicroseconds(uint64_t delayTime){
    // Starting time:
    uint64_t startTime = micros();

    // Wait while reading button "S":
    while (startTime + delayTime > micros()){}
}