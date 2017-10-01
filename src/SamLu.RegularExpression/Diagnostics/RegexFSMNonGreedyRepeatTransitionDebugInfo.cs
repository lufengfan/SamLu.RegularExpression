﻿using SamLu.RegularExpression.StateMachine;
using SamLu.RegularExpression.StateMachine.FunctionalTransitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Diagnostics
{
    /// <summary>
    /// 为 <see cref="RegexFSMNonGreedyRepeatTransition{T}"/> 提供调试信息。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    public class RegexFSMNonGreedyRepeatTransitionDebugInfo<T> : RegexFSMFunctionalTransitionDebugInfoBase<T, RegexFSMNonGreedyRepeatTransition<T>>
    {
        /// <summary>
        /// 获取 <see cref="RegexFSMNonGreedyRepeatTransition{T}"/> 的显式名称。
        /// </summary>
        protected override string Name => "non-greedy";

        /// <summary>
        /// 获取 <see cref="RegexFSMNonGreedyRepeatTransition{T}"/> 的显式参数序列。
        /// </summary>
        protected override IEnumerable<string> Parameters => null;

        /// <summary>
        /// 使用规范参数列表初始化 <see cref="RegexFSMNonGreedyRepeatTransition{T}"/> 类的新实例。
        /// </summary>
        /// <param name="functionalTransition">正则表达式构造的有限状态机的功能转换。</param>
        /// <param name="args">获取调试信息的参数列表。</param>
        public RegexFSMNonGreedyRepeatTransitionDebugInfo(RegexFSMNonGreedyRepeatTransition<T> functionalTransition, params object[] args) : base(functionalTransition, args) { }
    }

    /// <summary>
    /// 为 <see cref="RegexFSMNonGreedyRepeatTransition{T, TState}"/> 提供调试信息。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    /// <typeparam name="TState">正则构造的有限状态机的状态的类型。</typeparam>
    public class RegexFSMNonGreedyRepeatTransitionDebugInfo<T, TState> : RegexFSMFunctionalTransitionDebugInfoBase<T, RegexFSMNonGreedyRepeatTransition<T, TState>>
        where TState : IRegexFSMState<T>
    {
        /// <summary>
        /// 获取 <see cref="RegexFSMNonGreedyRepeatTransition{T, TState}"/> 的显式名称。
        /// </summary>
        protected override string Name => "non-greedy";

        /// <summary>
        /// 获取 <see cref="RegexFSMNonGreedyRepeatTransition{T, TState}"/> 的显式参数序列。
        /// </summary>
        protected override IEnumerable<string> Parameters => null;

        /// <summary>
        /// 使用规范参数列表初始化 <see cref="RegexFSMNonGreedyRepeatTransition{T, TState}"/> 类的新实例。
        /// </summary>
        /// <param name="functionalTransition">正则表达式构造的有限状态机的功能转换。</param>
        /// <param name="args">获取调试信息的参数列表。</param>
        public RegexFSMNonGreedyRepeatTransitionDebugInfo(RegexFSMNonGreedyRepeatTransition<T, TState> functionalTransition, params object[] args) : base(functionalTransition, args) { }
    }
}
