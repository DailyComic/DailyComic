# DailyComic
Little tool for delivering a daily strip of your favourite comic to your communication platform

# Is it live?
Yes

# Is it free?
Yes

# Is it guaranteed?
No

# How do I get it for my Teams/Slack?
Go to https://dailycomic.github.io/ and subscribe (no registration needed)

# How does it work?
There's an Azure Function App with a set of functions which run on an interval and send comic strips to subscribers.
The Azure Function uses an incoming webhook to send a simple json payload to the communication platform.

# Why does it exist?
I wanted to create a 'small but functional' side projects after having done my MSFT Azure training.

# Anything else?
Have a look at [Wiki](https://github.com/DailyComic/DailyComic/wiki) for more info, especially on copyrights, guarantees and security
