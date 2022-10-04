#Requires AutoHotkey v2.0-beta

;WINKEY + W - Runs Chrome
#w::
    {
        Run "chrome"
        return
    }
;WINKEY + C - Runs Visual Studio Code
#c::
    {
        Run "code"
        return
    }

;WINKEY + Q - Kills current window
#q::
    {
        PID := WinGetPID("ahk_id " WinExist("A"))
        ProcessClose(PID)
        Return
    }


$~LWin::
{
    
    SendInput "#r"
    Return
}



;WORKAROUND FOR WINKEY SHORTCUTSS
#1::
    {
        SendInput "#!^1"
        Return
    }
#2::
    {
        SendInput "#!^2"
        Return
    }
#3::
    {
        SendInput "#!^3"
        Return
    }
#4::
    {
        SendInput "#!^4"
        Return
    }
#5::
    {
        SendInput "#!^5"
        Return
    }
#6::
    {
        SendInput "#!^6"
        Return
    }
#7::
    {
        SendInput "#!^7"
        Return
    }
#8::
    {
        SendInput "#!^8"
        Return
    }
#9::
    {
        SendInput "#!^9"
        Return
    }
#0::
    {
        SendInput "#!^0"
        Return
    }

