# discord-points-bot ![.NET Core 3 Build](https://github.com/a-sink-a-wait/discord-points-bot/workflows/.NET%20Core%203%20Build/badge.svg?branch=master) ![.NET Core RC](https://github.com/a-sink-a-wait/discord-points-bot/workflows/.NET%20Core%20RC/badge.svg?branch=master&event=push)
A derpy bot for Discord. Give and take arbitrary points from channel members :)

## Bot
The Bot project runs in a continous Azure Web Job. Some configuration needed.

## CLI
Small project for directly dropping messages on the command queue.

## Function
An Azure function with a service bus trigger. Reads from an event feed in Cosmos and updates a readmodel in Table Storage.
Also has HTTP triggers to handle Query side.

## Infrastructure
Holds dependencies related to infrastructure. Right now it has implementations for communication with Azure Service Bus and some contracts used in the Command and Query side of the application. Holds any external integration implementations (Service Bus / Cosmos).
