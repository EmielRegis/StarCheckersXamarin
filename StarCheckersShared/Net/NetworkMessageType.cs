using System;

namespace StarCheckersWindows
{
    public enum NetworkMessageType
    {
        OK = 2,
        FAIL = 4,
        TIMEOUT = 8,
        CILENT_CONNECTION = 16,
        SERVER_CONNECTED = 32,
        CLIENT_DETACHED = 64,
        CONNECTION_LOST = 128,
        FIND_PAIR = 256,
        INVITE = 512,
        INVITATION_ACCEPTED = 1024,
        PAIR_FOUND = 2048,
        START_GAME = 4096,
        END_GAME = 8192,
        MOVE = 16384,
        ATTACK = 32768,
        MULTIPLE_ATTACK = 65536
    }
}

