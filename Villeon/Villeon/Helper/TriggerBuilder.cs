using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;

namespace Villeon.Helper
{
    public enum TriggerID
    {
        PLAYER,
        ENEMY,
        ATTACKLEFT,
        ATTACKRIGHT,
    }

    public static class TriggerBuilder
    {
        public static Trigger Build(TriggerType type, Vector2 offset, float width, float height)
        {
            Trigger trigger = new Trigger(type, offset, width, height);

            switch (type)
            {
                case TriggerType.DAMAGE:
                    trigger.Damage = 50;
                    trigger.Time = 0.3f;
                    trigger.Impulse = new Vector2(5000, 0);
                    break;
            }

            return trigger;
        }

        public static Trigger Build(TriggerID id)
        {
            Trigger trigger;

            switch (id)
            {
                case TriggerID.PLAYER:
                    trigger = new Trigger(TriggerType.MOB, new Vector2(-0.5f, 0), 1f, 1f);
                    break;

                case TriggerID.ENEMY:
                    trigger = new Trigger(TriggerType.MOB, new Vector2(-0.50f, 0), 1f, 1f);
                    break;

                case TriggerID.ATTACKLEFT:
                    trigger = new Trigger(TriggerType.DAMAGE, new Vector2(0.5f, -0.5f), 0.5f, 0.5f);
                    trigger.Damage = 5;
                    trigger.Time = 0.3f;
                    trigger.Impulse = new Vector2(-5000, 0);
                    break;

                case TriggerID.ATTACKRIGHT:
                    trigger = new Trigger(TriggerType.DAMAGE, new Vector2(-2f, -0.5f), 0.5f, 0.5f);
                    trigger.Damage = 5;
                    trigger.Time = 0.3f;
                    trigger.Impulse = new Vector2(5000, 0);
                    break;

                default:
                    trigger = new Trigger(TriggerType.DAMAGE, new Vector2(0, 0), 0, 0);
                    break;
            }

            return trigger;
        }
    }
}
