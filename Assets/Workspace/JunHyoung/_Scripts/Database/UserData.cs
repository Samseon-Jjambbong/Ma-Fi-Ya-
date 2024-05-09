using System;

//직렬화 가능한것만 Json 으로 만들 수 있음 
[Serializable]
public class UserData
{
    public string Name;
    public int playCount;
    public int winCount;
    public int level;

    public UserData() { this.Name = "nickName"; this.playCount = 0; this.winCount = 0;  this.level = 1;  }

    public UserData( string nickName)
    {
        this.Name = nickName;
        this.playCount = 0;
        this.winCount = 0;
        this.level = 1;
    }
}