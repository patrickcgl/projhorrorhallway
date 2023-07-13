int Sensor1; // Oben-Rechts
int Sensor2; // Unten
int Sensor3; // Oben-Links

int StickButton = 3; // Button
int JoyStick_X = A4; // X-axis signal
int JoyStick_Y = A5; // Y-axis signal
 
void setup ()
{
  pinMode (JoyStick_X, INPUT);
  pinMode (JoyStick_Y, INPUT);
  pinMode (StickButton, INPUT);

  digitalWrite(StickButton, HIGH);

  Serial.begin (9600); // Serial output with 9600 bps
}
 

void loop ()
{
  float x, y;
  int button;

  Sensor1 = analogRead(A0);
  Sensor2 = analogRead(A1);
  Sensor3 = analogRead(A2);

  x = analogRead (JoyStick_X) * (5.0 / 1023.0); 
  y = analogRead (JoyStick_Y) * (5.0 / 1023.0);
  button = digitalRead (StickButton);

  //... and output at this position
  Serial.print(x, 4);Serial.print ("|");Serial.print(y, 4);Serial.print("|");
 
  if(button==1)
  {
      Serial.print("0");
  }
  else
  {
      Serial.print("1");
  }

  Serial.print("|");

  Serial.print(Sensor1);

  Serial.print("|");

  Serial.print(Sensor2);

  Serial.print ("|");

  Serial.print(Sensor3);

  Serial.println("");
}