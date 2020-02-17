using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.InGame.Bid
{
    public class BidUserCondition 
    {
        public int ItemEffectsId = 0;
        public string ItemCondition = "";
        public int ItemValue = 0;
        public bool Checked = false;

        public BidUserCondition(int itemeffects,string itemcondition,int itemvalue)
        {
            ItemEffectsId = itemeffects;
            ItemCondition = itemcondition;
            ItemValue = itemvalue;
        }

        public bool BidConditionChecker(int ItemValueToCheck)
        {
            Checked = true;
            if (ItemCondition != "")
            {
                if(ItemCondition == "=" || ItemCondition == "==")
                {
                    if (ItemValueToCheck == ItemValue)
                        return true;
                }
                else if(ItemCondition == ">")
                {
                    if (ItemValueToCheck > ItemValue)
                        return true;
                }
                else if (ItemCondition == "<")
                {
                    if (ItemValueToCheck < ItemValue)
                        return true;
                }
                else if (ItemCondition == ">=")
                {
                    if (ItemValueToCheck >= ItemValue)
                        return true;
                }
                else if (ItemCondition == "<=")
                {
                    if (ItemValueToCheck <= ItemValue)
                    {
                        return true;
                    }
                }
                else if (ItemCondition == "!=" || ItemCondition == "=!")
                {
                    if (ItemValueToCheck != ItemValue)
                        return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

    }
}
