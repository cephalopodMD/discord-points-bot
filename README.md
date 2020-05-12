# discord-points-bot
A derpy bot for Discord. Give and take arbitrary points from channel members :)

## Bot
The Bot project runs in a continous Azure Web Job. Some configuration needed.

## CLI
Small project for directly dropping messages on the command queue.

## CommandsFunction
An Azure function with a service bus trigger. Reads commands off of queue and updates the blob store.

## Core
Core project that holds dependencies for the `CLI` and `Bot` projects. Wraps the Service Bus infrastructure.
