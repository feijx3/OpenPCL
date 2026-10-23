using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace MeloongCore
{
    public class ProgressProvider
    {
        [TupleElementNames(new string[] { "actual", "skiped" })]
        [Nullable(new byte[] { 2, 0 })]
        [field: TupleElementNames(new string[] { "actual", "skiped" })]
        [field: Nullable(new byte[] { 2, 0 })]
        public event Action<ValueTuple<double, double>> ProgressChanged;

        [Nullable(2)]
        [method: NullableContext(2)]
        [field: Nullable(2)]
        private event Action ChildProgressChanged;

        private void InvokeProgressChanged()
        {
            Action<ValueTuple<double, double>> progressChanged = this.ProgressChanged;
            if (progressChanged != null)
            {
                progressChanged(this.GetRaw());
            }
            Action childProgressChanged = this.ChildProgressChanged;
            if (childProgressChanged == null)
            {
                return;
            }
            childProgressChanged();
        }

        private double progressSum
        {
            get
            {
                return this.progressParts.Item1 + this.progressParts.Item2 + this.progressParts.Item3;
            }
        }

        private bool _Set(double value, bool skiped)
        {
            value = value.Clamp(0.0, 1.0);
            double num = value - this.progressSum;
            if (num < 0.0)
            {
                this.progressParts = new ValueTuple<double, double, double>(this.progressParts.Item1 * value / this.progressSum, this.progressParts.Item2 * value / this.progressSum, this.progressParts.Item3 * value / this.progressSum);
            }
            else if (skiped)
            {
                this.progressParts.Item2 = this.progressParts.Item2 + num;
            }
            else
            {
                this.progressParts.Item1 = this.progressParts.Item1 + num;
            }
            return num != 0.0;
        }

        public void Set(double value, bool skiped = false, ProgressProvider.ChildrenAction action = ProgressProvider.ChildrenAction.Finished)
        {
            bool flag2;
            lock (this)
            {
                flag2 = this._Set(value, skiped);
                flag2 |= this.AssumeChildrenAs(action);
            }
            if (flag2)
            {
                this.InvokeProgressChanged();
            }
        }

        public void Add(double value, bool skiped = false, ProgressProvider.ChildrenAction action = ProgressProvider.ChildrenAction.Finished)
        {
            bool flag2;
            lock (this)
            {
                flag2 = this._Set(value + this.progressSum, skiped);
                flag2 |= this.AssumeChildrenAs(action);
            }
            if (flag2)
            {
                this.InvokeProgressChanged();
            }
        }

        public void Finish(ProgressProvider.ChildrenAction action = ProgressProvider.ChildrenAction.Finished)
        {
            this.Set(1.0, false, action);
        }

        public void Skip(ProgressProvider.ChildrenAction action = ProgressProvider.ChildrenAction.Skiped)
        {
            this.Set(1.0, true, action);
        }

        public void Reset()
        {
            bool flag2;
            lock (this)
            {
                flag2 = this.progressSum > 0.0;
                this.progressParts = new ValueTuple<double, double, double>(0.0, 0.0, 0.0);
                this.childrens.ForEach(delegate([TupleElementNames(new string[] { "percentage", "child" })] [Nullable(new byte[] { 0, 1 })] ValueTuple<double, ProgressProvider> c)
                {
                    c.Item2.ChildProgressChanged -= this.InvokeProgressChanged;
                });
                this.childrens.Clear();
            }
            if (flag2)
            {
                this.InvokeProgressChanged();
            }
        }

        [NullableContext(1)]
        public ProgressProvider SplitTo(double value)
        {
            ProgressProvider progressProvider;
            lock (this)
            {
                progressProvider = this.SplitBy(new double[] { value - this.progressSum }).First<ProgressProvider>();
            }
            return progressProvider;
        }

        [NullableContext(1)]
        public List<ProgressProvider> SplitBy(params double[] percentages)
        {
            List<ProgressProvider> list;
            lock (this)
            {
                if (percentages.Any<double>((double p) => p <= 0.0))
                {
                    throw new ArgumentException("子项进度必须为正数且");
                }
                this.progressParts.Item3 = this.progressParts.Item3 + ((percentages.Sum() + this.progressSum).Clamp(0.0, 1.0) - this.progressSum);
                list = percentages.Select<double, ProgressProvider>(delegate(double percentage)
                {
                    ProgressProvider progressProvider = new ProgressProvider();
                    progressProvider.ChildProgressChanged += this.InvokeProgressChanged;
                    this.childrens.Add(new ValueTuple<double, ProgressProvider>(percentage, progressProvider));
                    return progressProvider;
                }).ToList<ProgressProvider>();
            }
            return list;
        }

        private bool AssumeChildrenAs(ProgressProvider.ChildrenAction action)
        {
            if (action == ProgressProvider.ChildrenAction.None)
            {
                return false;
            }
            bool changed2;
            lock (this)
            {
                bool changed = false;
                if (this.childrens.Any<ValueTuple<double, ProgressProvider>>(([TupleElementNames(new string[] { "percentage", "child" })] ValueTuple<double, ProgressProvider> c) => c.Item1 > 0.0))
                {
                    double mult = this.progressParts.Item3 / this.childrens.Sum<ValueTuple<double, ProgressProvider>>(([TupleElementNames(new string[] { "percentage", "child" })] ValueTuple<double, ProgressProvider> c) => c.Item1);
                    this.childrens.ForEach(delegate([TupleElementNames(new string[] { "percentage", "child" })] [Nullable(new byte[] { 0, 1 })] ValueTuple<double, ProgressProvider> c)
                    {
                        ValueTuple<double, double> raw = c.Item2.GetRaw();
                        double item = raw.Item1;
                        double item2 = raw.Item2;
                        ProgressProvider <>4__this = this;
                        <>4__this.progressParts.Item1 = <>4__this.progressParts.Item1 + mult * c.Item1 * ((action == ProgressProvider.ChildrenAction.Finished) ? (1.0 - item2) : item);
                        ProgressProvider <>4__this2 = this;
                        <>4__this2.progressParts.Item2 = <>4__this2.progressParts.Item2 + mult * c.Item1 * ((action == ProgressProvider.ChildrenAction.Skiped) ? (1.0 - item) : item2);
                        c.Item2.ChildProgressChanged -= this.InvokeProgressChanged;
                        if (item + item2 < 0.9999999)
                        {
                            changed = true;
                        }
                    });
                }
                this.progressParts.Item3 = 0.0;
                this.childrens.Clear();
                changed2 = changed;
            }
            return changed2;
        }

        [return: TupleElementNames(new string[] { "actual", "skiped" })]
        public ValueTuple<double, double> GetRaw()
        {
            bool flag = false;
            ValueTuple<double, double> current2;
            try
            {
                Monitor.Enter(this, ref flag);
                ValueTuple<double, double> current = new ValueTuple<double, double>(this.progressParts.Item1, this.progressParts.Item2);
                if (this.childrens.Any<ValueTuple<double, ProgressProvider>>(([TupleElementNames(new string[] { "percentage", "child" })] ValueTuple<double, ProgressProvider> c) => c.Item1 > 0.0))
                {
                    double mult = this.progressParts.Item3 / this.childrens.Sum<ValueTuple<double, ProgressProvider>>(([TupleElementNames(new string[] { "percentage", "child" })] ValueTuple<double, ProgressProvider> c) => c.Item1);
                    this.childrens.ForEach(delegate([TupleElementNames(new string[] { "percentage", "child" })] [Nullable(new byte[] { 0, 1 })] ValueTuple<double, ProgressProvider> c)
                    {
                        ValueTuple<double, double> raw = c.Item2.GetRaw();
                        double item = raw.Item1;
                        double item2 = raw.Item2;
                        current.Item1 += mult * c.Item1 * item;
                        current.Item2 += mult * c.Item1 * item2;
                    });
                }
                current2 = current;
            }
            finally
            {
                if (flag)
                {
                    Monitor.Exit(this);
                }
            }
            return current2;
        }

        public bool IsFinished
        {
            get
            {
                return this.GetRaw().Item1 + this.GetRaw().Item2 > 0.9999999;
            }
        }

        public double GetIncreasing()
        {
            double num;
            lock (this)
            {
                ValueTuple<double, double> raw = this.GetRaw();
                ValueTuple<double, double> valueTuple = raw;
                ValueTuple<double, double> valueTuple2 = this.observedProgress;
                if (valueTuple.Item1 == valueTuple2.Item1 && valueTuple.Item2 == valueTuple2.Item2)
                {
                    num = this.incrementProgress;
                }
                else
                {
                    if (this.IsFinished)
                    {
                        this.incrementProgress = 1.0;
                    }
                    else if (raw.Item1 >= this.observedProgress.Item1)
                    {
                        this.incrementProgress += (1.0 - this.incrementProgress) * (raw.Item1 - this.observedProgress.Item1) / (1.0 - this.observedProgress.Item1 - raw.Item2);
                    }
                    this.observedProgress = raw;
                    num = this.incrementProgress;
                }
            }
            return num;
        }

        [TupleElementNames(new string[] { "actual", "skiped", "splited" })]
        private ValueTuple<double, double, double> progressParts = new ValueTuple<double, double, double>(0.0, 0.0, 0.0);

        [TupleElementNames(new string[] { "percentage", "child" })]
        [Nullable(new byte[] { 1, 0, 1 })]
        private readonly List<ValueTuple<double, ProgressProvider>> childrens = new List<ValueTuple<double, ProgressProvider>>();

        [TupleElementNames(new string[] { "actual", "skiped" })]
        private ValueTuple<double, double> observedProgress = new ValueTuple<double, double>(0.0, 0.0);

        private double incrementProgress;

        public enum ChildrenAction
        {
            None,
            Finished,
            Skiped
        }
    }
}
