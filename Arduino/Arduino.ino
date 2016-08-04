#include <SerialCommand.h>

SerialCommand sCmd;

int soundPin = 10;

int redLedPin = 8;
int redLedState = LOW;
long redPreviousMillis = 0;
long redBlinkInterval = 350;

int greenLedPin = 9;
int greenLedState = LOW;
long greenPreviousMillis = 0;
long greenBlinkInterval = 500;

bool isInAlert = false;

void setup()
{
  Serial.begin(9600);
  while (!Serial);

  sCmd.addCommand("PING", pingHandler);
  sCmd.addCommand("ECHO", echoHandler);
  sCmd.addCommand("START_ALERT", StartAlert);
  sCmd.addCommand("STOP_ALERT", StopAlert);
  sCmd.addDefaultHandler (unrecognized);

  pinMode (redLedPin, OUTPUT);
  pinMode (greenLedPin, OUTPUT);
  ResetVFX ();
}

void StartAlert ()
{
  noTone (soundPin);
  isInAlert = true;
  ResetVFX ();
}

void StopAlert ()
{
  noTone (soundPin);
  isInAlert = false;
  ResetVFX ();
  tone (soundPin, 100, 2000);
}

void ResetVFX ()
{
  digitalWrite (greenLedPin, LOW);
  digitalWrite (redLedPin, LOW);
}

void loop()
{
  if (Serial.available() > 0)
    sCmd.readSerial();

  delay(50);

  if (isInAlert)
    AlertFX ();

  if (!isInAlert)
    NormalFX ();
}

void NormalFX ()
{
  unsigned long currentMillis = millis(); 
  
  if (currentMillis - greenPreviousMillis > greenBlinkInterval)
  {
    greenPreviousMillis = currentMillis;

    if (greenLedState == LOW)
    {
      digitalWrite (greenLedPin, HIGH);
      greenLedState = HIGH;
    }
    else
    {
      ResetVFX ();
      greenLedState = LOW;
    }
  }
}

void AlertFX ()
{
  unsigned long currentMillis = millis(); 
  
  if (currentMillis - redPreviousMillis > redBlinkInterval)
  {
    redPreviousMillis = currentMillis;

    if (redLedState == LOW)
    {
      digitalWrite (redLedPin, HIGH);
      tone (soundPin, 2500, redBlinkInterval);
      redLedState = HIGH;
    }
    else
    {
      ResetVFX ();
      redLedState = LOW;
    }
  }
}

void pingHandler ()
{
  Serial.println("PONG");
}

void echoHandler ()
{
  char *arg;
  arg = sCmd.next();
  if (arg != NULL)
    Serial.println(arg);
  else
    Serial.println("nothing to echo");
}

void unrecognized(const char *command)
{
  Serial.println("What?");
}
