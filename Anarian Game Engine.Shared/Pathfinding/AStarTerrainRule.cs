using Anarian.Enumerators;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anarian.Pathfinding
{
    public struct AStarTerrainRule
    {
        public RuleTypeAppliedTo AppliedTo;
        public RuleMeasureType Measurement;
        public Comparison Comparison;
        public float Height;
        public int Amount;

        public AStarTerrainRule(RuleTypeAppliedTo appliedTo, RuleMeasureType measurementType, Comparison comparison, float height, int amount)
        {
            AppliedTo = appliedTo;
            Measurement = measurementType;
            Comparison = comparison;
            Height = height;
            Amount = amount;
        }

        public static AStarTerrainRule PassableRule (RuleMeasureType measurementType, Comparison comparison, int height)
        {
            return new AStarTerrainRule(RuleTypeAppliedTo.Passable, measurementType, comparison, height, 0);
        }
        public static AStarTerrainRule ImPassableRule (RuleMeasureType measurementType, Comparison comparison, int height)
        {
            return new AStarTerrainRule(RuleTypeAppliedTo.Impassable, measurementType, comparison, height, 0);
        }
        public static AStarTerrainRule BaseHeightRule (RuleMeasureType measurementType, Comparison comparison, float height, int amount)
        {
            return new AStarTerrainRule(RuleTypeAppliedTo.BaseMovementCost, measurementType, comparison, height, amount);
        }
    }
}
