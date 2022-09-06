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
  digitalWrite(dirCarrPin, HIGH);
  while(digitalRead(3)){
    PORTD = PORTD | B00010000;
    waitMicroseconds(speedDelay);
    PORTD = PORTD & B11101111;
    waitMicroseconds(speedDelay);
  }
}

void loop(){
  digitalWrite(dirCarrPin, LOW);
  for(int i = 0; i < 8000; i++){
    PORTD = PORTD | B00010000;
    waitMicroseconds(speedDelay);
    PORTD = PORTD & B11101111;
    waitMicroseconds(speedDelay);
  }
  digitalWrite(dirCarrPin, HIGH);
  while(digitalRead(3)){
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
