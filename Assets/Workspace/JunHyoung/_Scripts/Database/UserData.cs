
using System;


//직렬화 가능한것만 Json 으로 만들 수 있음 
[Serializable]
public class UserData
{
    public string nickName;
    public int playCount;
    public int WinCount;
    public int level;

    public UserData() { this.nickName = "nickName"; this.playCount = 0; this.WinCount = 0;  this.level = 1;  }

    public UserData( string nickName)
    {
        this.nickName = nickName;
        this.playCount = 0;
        this.WinCount = 0;
        this.level = 1;
    }
}