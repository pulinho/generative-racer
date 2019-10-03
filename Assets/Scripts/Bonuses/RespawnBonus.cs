using UnityEngine;

public class RespawnBonus : BonusBase
{
    public GameManager gm;

    public override void PerformBonus()
    {
        var spawns = 0;

        foreach (var player in gm?.players)
        {
            if(player.IsActive() && !player.isAlive)
            {
                var pos = transform.parent.position;
                pos.y += 8f;

                if(spawns > 0)
                {
                    // works for 3 spawns lol
                    pos.x += (spawns * 2 - 3) * 3f;
                }
                spawns++;

                player.Respawn(pos);
            }
        }

        gm?.SetMusicParameter(0); //
    }
}
