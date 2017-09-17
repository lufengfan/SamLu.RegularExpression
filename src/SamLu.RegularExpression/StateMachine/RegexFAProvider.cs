﻿using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    public class RegexFAProvider<T> : IRegexFAProvider<T>
    {
        private IRegexStateMachineActivationContextInfo<T> contextInfo;
        public IRegexStateMachineActivationContextInfo<T> ContextInfo => this.contextInfo;

        public RegexFAProvider(IRegexStateMachineActivationContextInfo<T> contextInfo)
        {
            if (contextInfo == null) throw new ArgumentNullException(nameof(contextInfo));

            this.contextInfo = contextInfo;
        }

        public IRegexFSM<T> GenerateRegexFSMFromRegexObject(RegexObject<T> regex, RegexOptions options)
        {
            if (regex == null) throw new ArgumentNullException(nameof(regex));

            BasicRegexNFA<T> nfa = this.contextInfo.ActivateRegexNFA();
            BasicRegexNFAState<T> startState = this.contextInfo.ActivateRegexNFAState();
            nfa.StartState = startState;

            BasicRegexFATransition<T, BasicRegexNFAState<T>> transition = this.GenerateNFATransitionFromRegexObject(regex, nfa, startState);
            BasicRegexNFAState<T> endState = this.contextInfo.ActivateRegexNFAState(true);

            nfa.SetTarget(transition, endState);

            return nfa;
        }

        protected virtual BasicRegexFATransition<T, BasicRegexNFAState<T>> GenerateNFATransitionFromRegexObject(
            RegexObject<T> regex,
            BasicRegexNFA<T> nfa,
            BasicRegexNFAState<T> state
        )
        {
            if (regex is RegexCondition<T> condition)
                return this.GenerateNFATransitionFromRegexCondition(condition, nfa, state);
            else if (regex is RegexRepeat<T> repeat)
                return this.GenerateNFATransitionFromRegexRepeat(repeat, nfa, state);
            else if (regex is RegexSeries<T> series)
            {
                BasicRegexNFAState<T> nextState = state;
                foreach (var item in series.Series)
                {
                    var transition = this.GenerateNFATransitionFromRegexObject(item, nfa, nextState);
                    nextState = this.contextInfo.ActivateRegexNFAState();
                    nfa.SetTarget(transition, nextState);
                }

                BasicRegexNFAEpsilonTransition<T> epsilonTransition = this.contextInfo.ActivateRegexNFAEpsilonTransition();
                nfa.AttachTransition(nextState, epsilonTransition);

                return epsilonTransition;
            }
            else if (regex is RegexParallels<T> parallels)
            {
                BasicRegexNFAState<T> endState = this.contextInfo.ActivateRegexNFAState();

                foreach (var item in parallels.Parallels)
                {
                    BasicRegexFATransition<T, BasicRegexNFAState<T>> transition = this.GenerateNFATransitionFromRegexObject(item, nfa, state);
                    nfa.AttachTransition(state, transition);
                }

                BasicRegexNFAEpsilonTransition<T> epsilonTransition = this.contextInfo.ActivateRegexNFAEpsilonTransition();
                nfa.AttachTransition(endState, epsilonTransition);

                return epsilonTransition;
            }
            else throw new NotSupportedException(string.Format("不支持的正则类型：{0}", regex.GetType()));
        }
        
        protected virtual BasicRegexFATransition<T, BasicRegexNFAState<T>> GenerateNFATransitionFromRegexCondition(
            RegexCondition<T> condition,
            BasicRegexNFA<T> nfa,
            BasicRegexNFAState<T> state
        )
        {
            BasicRegexFATransition<T, BasicRegexNFAState<T>> transition = this.contextInfo.ActivateRegexNFATransitionFromRegexCondition(condition);
            nfa.AttachTransition(state, transition);

            return transition;
        }

        protected virtual BasicRegexFATransition<T, BasicRegexNFAState<T>> GenerateNFATransitionFromRegexRepeat(
            RegexRepeat<T> repeat,
            BasicRegexNFA<T> nfa,
            BasicRegexNFAState<T> state
        )
        {
            ulong count;
            if (repeat.IsInfinte)
                count = repeat.MinimumCount ?? ulong.MinValue;
            else
                count = repeat.MaximumCount.Value;

            var transition = this.GenerateNFATransitionFromRegexRepeatInternal(repeat.InnerRegex, nfa, state, count, out IList<BasicRegexNFAState<T>> nodes);

            if (repeat.IsInfinte)
            {
                BasicRegexNFAEpsilonTransition<T> epsilonTransition = this.contextInfo.ActivateRegexNFAEpsilonTransition();
                var reversedNodes = nodes.Reverse();
                nfa.AttachTransition(reversedNodes.First(), epsilonTransition);
                nfa.SetTarget(epsilonTransition, reversedNodes.Skip(1).First());
            }
            else
            {
                if (count > 0)
                {
                    BasicRegexNFAEpsilonTransition<T> epsilonTransition = this.contextInfo.ActivateRegexNFAEpsilonTransition();
                    foreach (var _state in nodes.Take((int)(repeat.MaximumCount.Value - (repeat.MinimumCount ?? ulong.MinValue))))
                        nfa.AttachTransition(_state, epsilonTransition);

                    nfa.SetTarget(epsilonTransition, nodes.Reverse().ElementAt((int)(repeat.MinimumCount ?? ulong.MinValue)));
                }
            }

            return transition;
        }

        private BasicRegexFATransition<T, BasicRegexNFAState<T>> GenerateNFATransitionFromRegexRepeatInternal(
            RegexObject<T> innerRegex,
            BasicRegexNFA<T> nfa,
            BasicRegexNFAState<T> state,
            ulong count,
            out IList<BasicRegexNFAState<T>> nodes)
        {
            nodes = new List<BasicRegexNFAState<T>> { state };

            BasicRegexNFAState<T> nextState = state;
            for (ulong index = ulong.MinValue; index < count; index++)
            {
                BasicRegexFATransition<T, BasicRegexNFAState<T>> transition = this.GenerateNFATransitionFromRegexObject(innerRegex, nfa, nextState);
                nextState = this.contextInfo.ActivateRegexNFAState();
                nfa.SetTarget(transition, nextState);

                nodes.Add(nextState);
            }

            BasicRegexNFAEpsilonTransition<T> epsilonTransition = this.contextInfo.ActivateRegexNFAEpsilonTransition();
            nfa.AttachTransition(nextState, epsilonTransition);

            return epsilonTransition;
        }
        
        public IRegexDFA<T> GenerateRegexDFAFromRegexFSM(IRegexFSM<T> nfa)
        {
            if (nfa == null) throw new ArgumentNullException(nameof(nfa));

            nfa.Optimize();

            BasicRegexDFA<T> dfa = new BasicRegexDFA<T>() { StartState = new BasicRegexDFAState<T>() };

            // 队列 Q 放置的是未被处理的已经创建了的 NFA 状态组（及其对应的 DFA 状态）。
            var Q = new Queue<(RegexFAStateGroup<T, IRegexFSMState<T>>, BasicRegexDFAState<T>)>();
            // 集合 C 放置的是已经存在的 NFA 状态组（及其对应的 DFA 状态）。
            var C = new Collection<(RegexFAStateGroup<T, IRegexFSMState<T>>, BasicRegexDFAState<T>)>();
#if false
            // 集合 D 放置的是处理后连接指定两个 DFA 状态的所有转换接受的对象的并集。
            var D = new Dictionary<(RegexDFAState<T>, RegexDFAState<T>), IList<ISet<T>>>();
#endif

            var startTuple = (new RegexFAStateGroup<T, IRegexFSMState<T>>(nfa.StartState), dfa.StartState);
            Q.Enqueue(startTuple);
            C.Add(startTuple);

            while (Q.Count != 0)
            {
                /* 从队列 Q 中取出一个状态。 */
                (var group, var dfaStateFrom) = Q.Dequeue();

                /* 计算从这个状态输出的所有转换及其所接受的对象集的并集。 */
                // 计算所有输出转换。
                var transitions = new HashSet<IRegexFSMTransition<T>>(
                        group.SelectMany(__state => __state.Transitions)
                    );
                /* 优化：构建转换/可接受对象集字典。 */
                var accreditedSetsDic = transitions
                    .ToDictionary(
                        (transition => transition),
                        (transition => this.contextInfo.GetAccreditedSetFromRegexNFATransition((BasicRegexFATransition<T,BasicRegexNFAState<T>>)transition))
                    );
                // 计算接受的对象集的并集。
                var sets = accreditedSetsDic.Values.ToArray();

                if (sets.Length > 1)
                {
                    /* 优化：对并集进行拆分，拆分成最小单位的字符集。 */
                    // 不需要拆分的字符集（与并集中的其他字符集都没有相交）。
                    var reservedSets = sets
                    //.Where(l_set => l_set != null)
                    .Where(l_set =>
                        sets
                            //.Where(r_set => r_set != null)
                            .Where(r_set => l_set != r_set)
                            .All(r_set => !l_set.Overlaps(r_set))
                    ).ToArray();
                    var elseSets = sets.Except(reservedSets).ToList();

                    if (elseSets.Count == 0) sets = reservedSets;
                    else if (elseSets.Count == 1) sets = reservedSets.Concat(elseSets).ToArray();
                    else
                    {
                        var combinations = Math.Combination.GetCombinationsWithRank(elseSets, 2);
                        var splitedSets = combinations
                            .SelectMany(combination =>
                            {
                                var l_set = combination[0];
                                var r_set = combination[1];

                                if (l_set.IsProperSubsetOf(r_set))
                                    return new[] { l_set, this.contextInfo.GetAccreditedSetExceptResult(r_set, l_set) };
                                else if (l_set.IsProperSupersetOf(r_set))
                                    return new[] { this.contextInfo.GetAccreditedSetExceptResult(l_set, r_set), r_set };
                                else
                                    return new[] { this.contextInfo.GetAccreditedSetUnionResult(l_set, r_set) };
                            });

                        sets = reservedSets.Concat(splitedSets).ToArray();
                    }
                }

                /* 然后对这个并集中的每一个对象集寻找接受其的转换，把这些转换的目标状态的并集 newGroup 计算出来。 */
                foreach (var set in sets)
                {
                    var newGroup = new RegexFAStateGroup<T, IRegexFSMState<T>>(new HashSet<IRegexFSMState<T>>(
                        accreditedSetsDic
                            .Where(pair => pair.Value.IsSupersetOf(set))
                            .Select(pair => pair.Key.Target)
                    ));

                    if (newGroup.Count == 0) continue;
                    else
                    {
                        (RegexFAStateGroup<T, IRegexFSMState<T>>, BasicRegexDFAState<T> dfaState)? tuple =
                            C
                                .Cast<(RegexFAStateGroup<T, IRegexFSMState<T>>, BasicRegexDFAState<T>)?>()
                                .FirstOrDefault(_tuple =>
                                {
                                    (RegexFAStateGroup<T, IRegexFSMState<T>> __nfaStateGroup, BasicRegexDFAState<T>) t = _tuple.Value;
                                    return t.__nfaStateGroup.Equals(newGroup);
                                });
                        BasicRegexDFAState<T> dfaStateTo;
                        if (tuple.HasValue)
                            // 如果 C 中含有获得接受了指定输入的 NFA 状态集。
                            dfaStateTo = tuple.Value.dfaState;
                        else
                        {
                            // 如果接受了指定输入的 NFA 状态集中有结束状态。
                            bool isTerminal = newGroup.Any(__state => __state.IsTerminal);
                            dfaStateTo = this.contextInfo.ActivateRegexDFAState(isTerminal);

                            // 将新状态集存入队列，进行后续处理。
                            var newTuple = (newGroup, dfaStateTo);
                            Q.Enqueue(newTuple);
                            C.Add(newTuple);
                        }

                        BasicRegexFATransition<T, BasicRegexDFAState<T>> dfaTransition = this.contextInfo.ActivateRegexDFATransitionFromAccreditedSet(set);
                        dfa.AttachTransition(dfaStateFrom, dfaTransition);
                        dfa.SetTarget(dfaTransition, dfaStateTo);

#if false
                        var __key = (dfaStateFrom, dfaStateTo);
                        IList<ISet<T>> __list;
                        if (!D.ContainsKey(__key))
                        {
                            __list = new List<ISet<T>>();
                            D.Add(__key, __list);
                        }
                        else __list = D[__key];
                        __list.Add(set);
#endif
                    }
                }
            }

#if false
            foreach (var pair in D)
            {
                (RegexDFAState<T> dfaStateFrom, RegexDFAState<T> dfaStateTo) = pair.Key;
                ISet<T> set;
                if (pair.Value.Count == 1) set = pair.Value[0];
                else set = pair.Value.Aggregate((s1, s2) => this.contextInfo.GetAccreditedSetUnionResult(s1, s2));

                RegexFATransition<T, RegexDFAState<T>> dfaTransition = this.contextInfo.ActivateRegexDFATransitionFromAccreditedSet(set);
                dfa.AttachTransition(dfaStateFrom, dfaTransition);
                dfa.SetTarget(dfaTransition, dfaStateTo);
            }
#endif

            return dfa;
        }
    }
}
