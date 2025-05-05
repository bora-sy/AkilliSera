#include <Arduino.h>
#include <WiFi.h>
#include <HTTPClient.h>

#include "secret.h"

const String BASE_SERVER_URL = "http://10.134.100.230:3000";

const int DATA_SEND_REPEAT_MS = 1000;

const int WIFI_TIMEOUT = 30000;

const int NODE_ID = 0;
const int PIN_SOLENOID = -1;
const int PIN_MOISTURE_SENSORS[] = {34};

enum Sound
{
  Begin,
  ConnectingToWifi,
  WiFiConnected,
  WiFiFailedToConnect
};

void PlaySound(Sound s)
{
  // TODO: Implement
}

bool WiFiInit()
{
  Serial.println("Connecting to WiFi");
  PlaySound(Sound::ConnectingToWifi);
  
  WiFi.mode(wifi_mode_t::WIFI_MODE_STA);

  WiFi.begin(WIFI_SSID, WIFI_PW);

  WiFi.waitForConnectResult(WIFI_TIMEOUT);


  if(WiFi.status() == wl_status_t::WL_CONNECTED)
  {
    Serial.println("WiFi Connected!");
    Serial.println("IP Address: " + WiFi.localIP().toString());
    PlaySound(Sound::WiFiConnected);
    return true;
  }

  Serial.printf("Failed to connect to WiFi // Status: %d\n", (int)WiFi.status());
  PlaySound(Sound::WiFiFailedToConnect);
  return false;
}

void setup() {
  Serial.begin(115200);
  Serial.println("App begin");


  const int relayPin = 25;
  const int nem1 = 33;
  const int nem2 = 32;

  pinMode(relayPin, OUTPUT);

  while(true)
  {
    int value1= analogRead(nem1);
    int value2= analogRead(nem2);
    //Serial.println(String(value1) + " " + String(value2));
    //delay(200);
    

    //delay(3000);
    digitalWrite(relayPin, HIGH);
    Serial.println("HIGH");
    delay(2000);
    digitalWrite(relayPin, LOW);
    Serial.println("LOW");
    delay(3000);
  }



  return;

  if(!WiFiInit()) ESP.restart();
}

int HTTPPost(const String url) {
  WiFiClient client;
  HTTPClient http;

  http.begin(client, url);
  int httpResponseCode = http.POST("{}");
  http.end();

  return httpResponseCode;
}


int GetMoistureLevel(int i)
{
  return analogRead(PIN_MOISTURE_SENSORS[i]);
}

bool SendData()
{
  Serial.println("Sending data...");
  int moistureLevelCount = sizeof(PIN_MOISTURE_SENSORS)/sizeof(PIN_MOISTURE_SENSORS[0]);

  int moistureLevel = 0;

  for(int i = 0; i<moistureLevelCount; i++)
  {
    moistureLevel += GetMoistureLevel(i);
  }

  moistureLevel /= moistureLevelCount;

  String url = BASE_SERVER_URL + "/greenhouse/nodedata?NodeID=" + String(NODE_ID) + "&humidValsStr=" + String(moistureLevel);

  int code = HTTPPost(url);

  if(code < 200 || code > 299)
  {

    Serial.printf("Failed to send data. [Status Code: %d]\n", code);
    return false;
  }

  Serial.println("Data sent successfully");
  return true;
}

ulong nextDataSend = 0;
void loop() {
  if(nextDataSend < millis())
  {
    bool suc = SendData();
    nextDataSend = millis() + DATA_SEND_REPEAT_MS;
  }
}