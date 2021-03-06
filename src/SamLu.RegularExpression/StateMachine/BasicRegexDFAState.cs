﻿using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    public class BasicRegexDFAStateBase<T> : DFAState, IRegexDFAState<T>
    {
        /// <summary>
        /// 获取 <see cref="BasicRegexDFAStateBase{T}"/> 的转换集。
        /// </summary>
        new public virtual ICollection<IRegexFSMTransition<T>> Transitions =>
            new ReadOnlyCollection<IRegexFSMTransition<T>>(base.Transitions.Cast<IRegexFSMTransition<T>>().ToList());

        /// <summary>
        /// 初始化 <see cref="BasicRegexDFAStateBase{T}"/> 类的新实例。
        /// </summary>
        public BasicRegexDFAStateBase() : base() { }

        /// <summary>
        /// 初始化 <see cref="BasicRegexDFAStateBase{T}"/> 类的新实例，该实例接受一个指定是否为结束状态的值。
        /// </summary>
        /// <param name="isTerminal">一个值，指示该实例是否为结束状态。</param>
        public BasicRegexDFAStateBase(bool isTerminal) : base(isTerminal) { }

        public BasicRegexDFAStateBase(BasicRegexDFAState<T> state) :
            this((state ?? throw new ArgumentNullException(nameof(state))).IsTerminal)
        {
            // 复制转换集合。
            foreach (var transition in state.Transitions)
                this.AttachTransition(transition);

            // 复制动作。
            this.EntryAction = state.EntryAction;
            this.ExitAction = state.ExitAction;
            this.InputAction = state.InputAction;
        }

        #region AttachTransition
        /// <summary>
        /// 添加指定的转换。
        /// </summary>
        /// <param name="transition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="transition"/> 的值为 null 。</exception>
        /// <exception cref="InvalidOperationException">在 <paramref name="transition"/> 为 <see cref="IEpsilonTransition"/> 接口的实例时抛出。试图向确定的有限自动机模型的状态中添加一个 ε 转换。</exception>
        public sealed override bool AttachTransition(ITransition transition) => this.AttachTransition((IAcceptInputTransition<T>)transition);

        /// <summary>
        /// 添加指定的转换。
        /// </summary>
        /// <param name="transition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="transition"/> 的值为 null 。</exception>
        /// <exception cref="InvalidOperationException">在 <paramref name="transition"/> 为 <see cref="IEpsilonTransition"/> 接口的实例时抛出。试图向确定的有限自动机模型的状态中添加一个 ε 转换。</exception>
        public bool AttachTransition(IRegexFSMTransition<T> transition) => this.AttachTransition((IAcceptInputTransition<T>)transition);

        /// <summary>
        /// 添加指定的接受输入转换。
        /// </summary>
        /// <param name="acceptInputTransition">要添加的接受输入转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="acceptInputTransition"/> 的值为 null 。</exception>
        /// <exception cref="InvalidOperationException">在 <paramref name="transition"/> 为 <see cref="IEpsilonTransition"/> 接口的实例时抛出。试图向确定的有限自动机模型的状态中添加一个 ε 转换。</exception>
        public virtual bool AttachTransition(IAcceptInputTransition<T> acceptInputTransition) => base.AttachTransition(acceptInputTransition);
        #endregion

        #region RemoveTransition
        /// <summary>
        /// 移除指定的转换。
        /// </summary>
        /// <param name="transition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="transition"/> 的值为 null 。</exception>
        /// <exception cref="InvalidOperationException">在 <paramref name="transition"/> 为 <see cref="IEpsilonTransition"/> 接口的实例时抛出。试图从确定的有限自动机模型的状态中移除一个 ε 转换。</exception>
        public sealed override bool RemoveTransition(ITransition transition) => this.RemoveTransition((IAcceptInputTransition<T>)transition);

        /// <summary>
        /// 移除指定的转换。
        /// </summary>
        /// <param name="transition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="transition"/> 的值为 null 。</exception>
        /// <exception cref="InvalidOperationException">在 <paramref name="transition"/> 为 <see cref="IEpsilonTransition"/> 接口的实例时抛出。试图从确定的有限自动机模型的状态中移除一个 ε 转换。</exception>
        public bool RemoveTransition(IRegexFSMTransition<T> transition) => this.RemoveTransition((IAcceptInputTransition<T>)transition);

        /// <summary>
        /// 移除指定的接受输入转换。
        /// </summary>
        /// <param name="acceptInputTransition">要添加的接受输入转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="acceptInputTransition"/> 的值为 null 。</exception>
        /// <exception cref="InvalidOperationException">在 <paramref name="transition"/> 为 <see cref="IEpsilonTransition"/> 接口的实例时抛出。试图从确定的有限自动机模型的状态中移除一个 ε 转换。</exception>
        public virtual bool RemoveTransition(IAcceptInputTransition<T> acceptInputTransition) => base.RemoveTransition(acceptInputTransition);
        #endregion
        
        #region IRegexFSMTransition{T} Implementation
        IEnumerable<IRegexFSMTransition<T>> IRegexFSMState<T>.GetOrderedTransitions() => this.GetOrderedTransitions();
        #endregion
    }

    /// <summary>
    /// 表示基础正则表达式（ Basic Regular Expression ）构造的确定的有限自动机的状态。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    public class BasicRegexDFAState<T> : DFAState<BasicRegexFATransition<T, BasicRegexDFAState<T>>>, IRegexDFAState<T, BasicRegexFATransition<T, BasicRegexDFAState<T>>>
    {
        /// <summary>
        /// 初始化 <see cref="BasicRegexDFAState{T}"/> 类的新实例。
        /// </summary>
        public BasicRegexDFAState() : base() { }

        /// <summary>
        /// 初始化 <see cref="BasicRegexDFAState{T}"/> 类的新实例，该实例接受一个指定是否为结束状态的值。
        /// </summary>
        /// <param name="isTerminal">一个值，指示该实例是否为结束状态。</param>
        public BasicRegexDFAState(bool isTerminal) : base(isTerminal) { }

        /// <summary>
        /// 添加指定的转换。
        /// </summary>
        /// <param name="transition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="transition"/> 的值为 null 。</exception>
        /// <exception cref="InvalidOperationException">在 <paramref name="transition"/> 为 <see cref="IEpsilonTransition"/> 接口的实例时抛出。试图向确定的有限自动机模型的状态中添加一个 ε 转换。</exception>
        public override bool AttachTransition(BasicRegexFATransition<T, BasicRegexDFAState<T>> transition)
        {
            if (transition == null) throw new ArgumentNullException(nameof(transition));

            if (transition is IEpsilonTransition)
                throw new InvalidOperationException(
                    "试图向确定的有限自动机模型的状态中添加一个 ε 转换。",
                    new ArgumentException("无法接受的 ε 转换。", nameof(transition))
                );

            return base.AttachTransition(transition);
        }

        /// <summary>
        /// 移除指定的转换。
        /// </summary>
        /// <param name="transition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="transition"/> 的值为 null 。</exception>
        /// <exception cref="InvalidOperationException">在 <paramref name="transition"/> 为 <see cref="IEpsilonTransition"/> 接口的实例时抛出。试图从确定的有限自动机模型的状态中移除一个 ε 转换。</exception>
        public override bool RemoveTransition(BasicRegexFATransition<T, BasicRegexDFAState<T>> transition)
        {
            if (transition == null) throw new ArgumentNullException(nameof(transition));

            if (transition is IEpsilonTransition)
                throw new InvalidOperationException(
                    "试图向确定的有限自动机模型的状态中添加一个 ε 转换。",
                    new ArgumentException("无法接受的 ε 转换。", nameof(transition))
                );

            return base.RemoveTransition(transition);
        }
        
        #region IRegexFSMTransition{T}/IRegexFSMTransition{T, TState} Implementation
        ICollection<IRegexFSMTransition<T>> IRegexFSMState<T>.Transitions =>
            new ReadOnlyCollection<IRegexFSMTransition<T>>(
                base.Transitions.Cast<IRegexFSMTransition<T>>().ToList()
            );

        ICollection<BasicRegexFATransition<T, BasicRegexDFAState<T>>> IRegexFSMState<T, BasicRegexFATransition<T, BasicRegexDFAState<T>>>.Transitions => throw new NotImplementedException();

        bool IRegexFSMState<T>.AttachTransition(IRegexFSMTransition<T> transition) =>
            base.AttachTransition((BasicRegexFATransition<T, BasicRegexDFAState<T>>)transition);

        bool IRegexFSMState<T>.RemoveTransition(IRegexFSMTransition<T> transition) =>
            base.RemoveTransition((BasicRegexFATransition<T, BasicRegexDFAState<T>>)transition);

        IEnumerable<BasicRegexFATransition<T, BasicRegexDFAState<T>>> IRegexFSMState<T, BasicRegexFATransition<T, BasicRegexDFAState<T>>>.GetOrderedTransitions() =>
            this.GetOrderedTransitions<T, BasicRegexDFAState<T>, BasicRegexFATransition<T, BasicRegexDFAState<T>>>();

        IEnumerable<IRegexFSMTransition<T>> IRegexFSMState<T>.GetOrderedTransitions() =>
            ((IRegexFSMState<T, BasicRegexFATransition<T, BasicRegexDFAState<T>>>)this).GetOrderedTransitions();

        bool IRegexDFAState<T>.AttachTransition(IAcceptInputTransition<T> acceptInputTransition) =>
            base.AttachTransition((BasicRegexFATransition<T, BasicRegexDFAState<T>>)acceptInputTransition);

        bool IRegexDFAState<T>.RemoveTransition(IAcceptInputTransition<T> acceptInputTransition) =>
            base.RemoveTransition((BasicRegexFATransition<T, BasicRegexDFAState<T>>)acceptInputTransition);
        #endregion
    }
}
