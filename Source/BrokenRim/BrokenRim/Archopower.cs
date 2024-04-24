using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace BrokenRim
{
    [StaticConstructorOnStartup]
    public class GeneGizmo_ResourceArchopower : GeneGizmo_Resource
    {
        // Token: 0x0600B791 RID: 46993 RVA: 0x0041A763 File Offset: 0x00418963
        public GeneGizmo_ResourceArchopower(Gene_Resource gene, List<IGeneResourceDrain> drainGenes, Color barColor, Color barHighlightColor) : base(gene, drainGenes, barColor, barHighlightColor)
        {
        }

        // Token: 0x0600B792 RID: 46994 RVA: 0x0041A77C File Offset: 0x0041897C
        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            GizmoResult result = base.GizmoOnGUI(topLeft, maxWidth, parms);
            float num = Mathf.Repeat(Time.time, 0.85f);
            float num2 = 1f;
            if (num < 0.1f)
            {
                num2 = num / 0.1f;
            }
            else if (num >= 0.25f)
            {
                num2 = 1f - (num - 0.25f) / 0.6f;
            }
            MainTabWindow_Inspect mainTabWindow_Inspect = (MainTabWindow_Inspect)MainButtonDefOf.Inspect.TabWindow;
            Command_Ability command_Ability;
            if ((command_Ability = (((mainTabWindow_Inspect != null) ? mainTabWindow_Inspect.LastMouseoverGizmo : null) as Command_Ability)) != null && this.gene.Max != 0f)
            {
                using (List<CompAbilityEffect>.Enumerator enumerator = command_Ability.Ability.EffectComps.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        CompAbilityEffect_ArchopowerCost compAbilityEffect_ArchopowerCost;
                        if ((compAbilityEffect_ArchopowerCost = (enumerator.Current as CompAbilityEffect_ArchopowerCost)) != null && compAbilityEffect_ArchopowerCost.Props.archopowerCost > 1E-45f)
                        {
                            Rect rect = this.barRect.ContractedBy(3f);
                            float width = rect.width;
                            float num3 = this.gene.Value / this.gene.Max;
                            rect.xMax = rect.xMin + width * num3;
                            float num4 = Mathf.Min(compAbilityEffect_ArchopowerCost.Props.archopowerCost / this.gene.Max, 1f);
                            rect.xMin = Mathf.Max(rect.xMin, rect.xMax - width * num4);
                            break;
                        }
                    }
                }
            }
            return result;
        }

        // Token: 0x0600B793 RID: 46995 RVA: 0x0041A95C File Offset: 0x00418B5C
        protected override void DrawHeader(Rect headerRect, ref bool mouseOverElement)
        {
            Gene_Archopower ArchopowerGene;
            if (this.IsDraggable && (ArchopowerGene = (this.gene as Gene_Archopower)) != null)
            {
                headerRect.xMax -= 24f;
                Rect rect = new Rect(headerRect.xMax, headerRect.y, 24f, 24f);
            }
            base.DrawHeader(headerRect, ref mouseOverElement);
        }

        // Token: 0x0600B794 RID: 46996 RVA: 0x0041AB0C File Offset: 0x00418D0C
        protected override string GetTooltip()
        {
            this.tmpDrainGenes.Clear();
            string text = string.Format("{0}: {1} / {2}\n", this.gene.ResourceLabel.CapitalizeFirst().Colorize(ColoredText.TipSectionTitleColor), this.gene.ValueForDisplay, this.gene.MaxForDisplay);
            if (this.gene.pawn.IsColonistPlayerControlled || this.gene.pawn.IsPrisonerOfColony)
            {
                if (this.gene.targetValue <= 0f)
                {
                    text += "NeverConsumeArchopower".Translate().ToString();
                }
                else
                {
                    text = text + ("ConsumeArchopowerBelow".Translate() + ": ") + this.gene.PostProcessValue(this.gene.targetValue);
                }
            }
            if (!this.drainGenes.NullOrEmpty<IGeneResourceDrain>())
            {
                float num = 0f;
                foreach (IGeneResourceDrain geneResourceDrain in this.drainGenes)
                {
                    if (geneResourceDrain.CanOffset)
                    {
                        this.tmpDrainGenes.Add(new Pair<IGeneResourceDrain, float>(geneResourceDrain, geneResourceDrain.ResourceLossPerDay));
                        num += geneResourceDrain.ResourceLossPerDay;
                    }
                }
                if (num != 0f)
                {
                    string text2 = (num < 0f) ? "RegenerationRate".Translate() : "DrainRate".Translate();
                    text = string.Concat(new string[]
                    {
                        text,
                        "\n\n",
                        text2,
                        ": ",
                        "PerDay".Translate(Mathf.Abs(this.gene.PostProcessValue(num))).Resolve()
                    });
                    foreach (Pair<IGeneResourceDrain, float> pair in this.tmpDrainGenes)
                    {
                        text = string.Concat(new string[]
                        {
                            text,
                            "\n  - ",
                            pair.First.DisplayLabel.CapitalizeFirst(),
                            ": ",
                            "PerDay".Translate(this.gene.PostProcessValue(-pair.Second).ToStringWithSign()).Resolve()
                        });
                    }
                }
            }
            if (!this.gene.def.resourceDescription.NullOrEmpty())
            {
                text = text + "\n\n" + this.gene.def.resourceDescription.Formatted(this.gene.pawn.Named("PAWN")).Resolve();
            }
            return text;
        }

        // Token: 0x04007213 RID: 29203
        private static readonly Texture2D ArchopowerCostTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.78f, 0.72f, 0.66f));

        // Token: 0x04007214 RID: 29204
        private const float TotalPulsateTime = 0.85f;

        // Token: 0x04007215 RID: 29205
        private List<Pair<IGeneResourceDrain, float>> tmpDrainGenes = new List<Pair<IGeneResourceDrain, float>>();
    }
}

class Gene_Archopower : Gene_Resource, IGeneResourceDrain
    {
        public Gene_Resource Resource
        {
            get
            {
                return this;
            }
        }

        public Pawn Pawn
        {
            get
            {
                return this.pawn;
            }
        }

        public string DisplayLabel
        {
            get
            {
                return this.Label + " (" + "Gene".Translate() + ")";
            }
        }

        public float ResourceLossPerDay
        {
            get
            {
                return this.def.resourceLossPerDay;
            }
        }

        public override float InitialResourceMax
        {
            get
            {
                return 1f;
            }
        }

        public override float MinLevelForAlert
        {
            get
            {
                return 0f;
            }
        }

        protected override Color BarColor
        {
            get
            {
                return new ColorInt(214, 221, 59).ToColor;
            }
        }

        public bool CanOffset
        {
            get
            {
                return this.Active;
            }
        }

        protected override Color BarHighlightColor
        {
            get
            {
                return new ColorInt(214, 221, 59).ToColor;
            }
        }

    }

    public class CompProperties_AbilityArchopowerCost : CompProperties_AbilityEffect
    {
        public CompProperties_AbilityArchopowerCost()
        {
            this.compClass = typeof(CompAbilityEffect_ArchopowerCost);
        }

        public override IEnumerable<string> ExtraStatSummary()
        {
            yield return "AbilityArchopowerCost".Translate() + ": " + Mathf.RoundToInt(this.archopowerCost * 100f);
            yield break;
        }

        public float archopowerCost;
    }

    public class CompAbilityEffect_ArchopowerCost : CompAbilityEffect
    {
        public new CompProperties_AbilityArchopowerCost Props
        {
            get
            {
                return (CompProperties_AbilityArchopowerCost)this.props;
            }
        }

        private bool HasEnoughArchopower
        {
            get
            {
                Pawn_GeneTracker genes = this.parent.pawn.genes;
                Gene_Archopower gene_Archopower = (genes != null) ? genes.GetFirstGeneOfType<Gene_Archopower>() : null;
                return gene_Archopower != null && gene_Archopower.Value >= this.Props.archopowerCost;
            }
        }

    }
