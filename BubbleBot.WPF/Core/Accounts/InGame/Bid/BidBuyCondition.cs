namespace BubbleBot.Core.Accounts.InGame.Bid
{
    public class BidBuyCondition
    {
        public string ItemCondition = "";
        public uint ItemEffectId;
        public int ItemValue;

        public BidBuyCondition(uint itemeffects, string itemcondition, int itemvalue)
        {
            ItemEffectId = itemeffects;
            ItemCondition = itemcondition;
            ItemValue = itemvalue;
        }

        public bool BidConditionChecker(uint ItemValueToCheck)
        {
            if (ItemCondition != "")
            {
                if (ItemCondition == "=" || ItemCondition == "==")
                {
                    if (ItemValueToCheck == ItemValue)
                        return true;
                }
                else if (ItemCondition == ">")
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
                    if (ItemValueToCheck <= ItemValue) return true;
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