using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;
using UnityEngine.Serialization;
using static SeekingProjectile;
using System.Xml.Linq;

namespace FFW_TT_BuffBlock
{
    class BuffSegmentMk2
    {
        public Tank tank;
        public BuffControllerMk2 controller;

        public Type effectComponent;
        public List<string> effectPath;

        public Dictionary<ModuleBuffMk2, int> effectBuffBlocks = new Dictionary<ModuleBuffMk2, int>();
        public Dictionary<object, float> effectMemory = new Dictionary<object, float>();

        private static MemberInfo GetMemberWithName(Type targetType, string name)
        {
            BindingFlags instanceFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            MemberInfo[] members = targetType.GetMembers(instanceFlags);
            foreach (MemberInfo member in members)
            {
                if (member.Name == name)
                {
                    return member;
                }
                object[] attributes = member.GetCustomAttributes(typeof(FormerlySerializedAsAttribute), true);
                foreach (object attribute in attributes)
                {
                    if (attribute is FormerlySerializedAsAttribute formerlySerializedAs && formerlySerializedAs.oldName == name)
                    {
                        BuffBlocks.logger.Warn($"🚨 Property '{name}' on type '{targetType}' has been renamed to '{member.Name}'");
                        return member;
                    }
                }
            }
            return null;
        }

        private static object GetValue(MemberInfo member, object instance)
        {
            object value = null;
            Type memberType = null;
            try
            {
                if (member.MemberType == MemberTypes.Property && member is PropertyInfo property)
                {
                    memberType = property.PropertyType;
                    if (property.CanRead)
                    {
                        value = property.GetValue(instance);
                        BuffBlocks.logger.Trace($"✔️ Got value of property {property.Name} as {value}");
                    }
                    else
                    {
                        BuffBlocks.logger.Error($"❌ Trying to get value of writeonly property {property.Name}");
                    }
                }
                else if (member.MemberType == MemberTypes.Field && member is FieldInfo field)
                {
                    member = field.FieldType;
                    value = field.GetValue(instance);
                    BuffBlocks.logger.Trace($"✔️ Got value of field {field.Name} as {value}");
                }
                else
                {
                    BuffBlocks.logger.Error($"❌ Trying to get value of non-gettable member {member.Name}");
                }
            }
            catch (Exception e)
            {
                BuffBlocks.logger.Error($"🛑 EXCEPTION WHEN GETTING VALUE");
                BuffBlocks.logger.Error(e);
            }
            if (value == null && memberType != null && memberType.IsValueType)
            {
                BuffBlocks.logger.Warn($"🚨 Using System.Activator because failed to get value somehow");
                value = Activator.CreateInstance(memberType);
            }
            return value;
        }

        private static void SetValue(MemberInfo member, object instance, object value)
        {
            try {
                if (member.MemberType == MemberTypes.Property && member is PropertyInfo property) {
                    if (property.CanWrite)
                    {
                        property.SetValue(instance, value);
                        BuffBlocks.logger.Trace($"✔️ Set value of property {property.Name} to {value}");
                    }
                    else
                    {
                        BuffBlocks.logger.Error($"❌ Trying to set value of readonly property {property.Name}");
                    }
                }
                else if (member.MemberType == MemberTypes.Field && member is FieldInfo field) {
                    field.SetValue(instance, value);
                    BuffBlocks.logger.Trace($"✔️ Set value of field {field.Name} to {value}");
                }
                else
                {
                    BuffBlocks.logger.Error($"❌ Trying to set value of non-settable member {member.Name}");
                }
            }
            catch (Exception e)
            {
                BuffBlocks.logger.Error($"🛑 EXCEPTION WHEN SETTING VALUE");
                BuffBlocks.logger.Error(e);
            }
        }

        public void ManipulateObj(List<TankBlock> blockPool, string request)
        {
            foreach (TankBlock block in blockPool)
            {
                BuffBlocks.logger.Trace("FFW! Manipulate Obj " + request + "... 1 ");
                /*if (request == "SAVE" && this.effectMemory.ContainsKey(block))
                {
                    BuffBlocks.logger.Trace("Aborting " + request + "! effectMemory already contains " + block.name);
                    return;
                }
                if ((request == "UPDATE" || request == "CLEAN") && !this.effectMemory.ContainsKey(block))
                {
                    BuffBlocks.logger.Trace("Aborting " + request + "! effectMemory doesn't contain " + block.name);
                    return;
                }*/
                object tgt = block.GetComponent(effectComponent);
                if (tgt == null)
                {
                    break;
                }
                if (request == "SAVE")
                {
                    this.effectMemory.Add(block, 1.0f);
                }

                List<object> lastIterObjs = null;
                List<object> thisIterObjs = new List<object> { tgt };

                MemberInfo member_lastIter = null;
                MemberInfo member_ThisIter = null;

                object structWarningObj = null;
                object structWarningParent = null;
                MemberInfo structWarningMember = null;

                foreach (string e in this.effectPath)
                {
                    member_lastIter = member_ThisIter;
                    lastIterObjs = new List<object>(thisIterObjs);
                    thisIterObjs = new List<object>();
                    foreach (object obj in lastIterObjs)
                    {
                        member_ThisIter = GetMemberWithName(obj.GetType(), e);
                        if (member_ThisIter != null)
                        {
                            object value_thisIter =  GetValue(member_ThisIter, obj);
                            if (value_thisIter != null)
                            {
                                var arrayTest = value_thisIter as Array;
                                var listTest = value_thisIter as System.Collections.IList;
                                Boolean isStruct = value_thisIter.GetType().IsValueType && !value_thisIter.GetType().IsPrimitive;
                                if (isStruct)
                                {
                                    structWarningObj = value_thisIter;
                                    structWarningParent = obj;
                                    structWarningMember = member_ThisIter;
                                    thisIterObjs.Add(structWarningObj);
                                }
                                else if (arrayTest != null)
                                {
                                    Array value_thisIterCasted = (Array)value_thisIter;
                                    foreach (object element in value_thisIterCasted)
                                    {
                                        thisIterObjs.Add(element);
                                    }
                                }
                                else if (listTest != null)
                                {
                                    System.Collections.IList value_thisIterCasted = (System.Collections.IList)value_thisIter;
                                    foreach (object element in value_thisIterCasted)
                                    {
                                        thisIterObjs.Add(element);
                                    }
                                }
                                else
                                {
                                    thisIterObjs.Add(value_thisIter);
                                }
                            }
                        }
                    }
                }

                foreach (object ara in lastIterObjs)
                {
                    if (member_ThisIter != null)
                    {
                        object value_thisIter =  GetValue(member_ThisIter, ara);
                        if (request == "SAVE")
                        {
                            BuffBlocks.logger.Trace("FFW! Saving " + value_thisIter);
                            if (value_thisIter.GetType() == typeof(float))
                            {
                                this.effectMemory[block] = (float)value_thisIter;
                            }
                            else if (value_thisIter.GetType() == typeof(int))
                            {
                                this.effectMemory[block] = Convert.ToSingle((int)value_thisIter);
                            }
                            else if (value_thisIter.GetType() == typeof(bool))
                            {
                                this.effectMemory[block] = Convert.ToSingle((bool)value_thisIter);
                            }
                        }
                        else if (request == "UPDATE")
                        {
                            BuffBlocks.logger.Trace("FFW! Update From " +  GetValue(member_ThisIter, ara));
                            if (value_thisIter.GetType() == typeof(float))
                            {
                                 SetValue(member_ThisIter, ara, this.effectMemory[block] * this.GetBuffAverage(block.name) + this.GetBuffAddAverage(block.name));
                            }
                            else if (value_thisIter.GetType() == typeof(int))
                            {
                                 SetValue(member_ThisIter, ara, Convert.ToInt32(Math.Ceiling(this.effectMemory[block] * this.GetBuffAverage(block.name) + this.GetBuffAddAverage(block.name))));
                            }
                            else if (value_thisIter.GetType() == typeof(bool))
                            {
                                 SetValue(member_ThisIter, ara, Convert.ToBoolean(Math.Round(BuffControllerMk2.Clamp(this.effectMemory[block] * this.GetBuffAverage(block.name) + this.GetBuffAddAverage(block.name), 0.0f, 1.0f))));
                            }
                            BuffBlocks.logger.Trace("FFW! Update To " +  GetValue(member_ThisIter, ara));
                        }
                        else if (request == "CLEAN")
                        {
                            BuffBlocks.logger.Trace("FFW! Clean From " +  GetValue(member_ThisIter, ara));
                            object valueSetTo = null;
                            if (value_thisIter.GetType() == typeof(float))
                            {
                                valueSetTo = this.effectMemory[block];
                                SetValue(member_ThisIter, ara, valueSetTo);
                            }
                            else if (value_thisIter.GetType() == typeof(int))
                            {
                                valueSetTo = Convert.ToInt32(Math.Ceiling(this.effectMemory[block]));
                                SetValue(member_ThisIter, ara, valueSetTo);
                            }
                            else if (value_thisIter.GetType() == typeof(bool))
                            {
                                valueSetTo = Convert.ToBoolean(Math.Round(BuffControllerMk2.Clamp(this.effectMemory[block], 0.0f, 1.0f)));
                                SetValue(member_ThisIter, ara, valueSetTo);
                            }
                            BuffBlocks.logger.Trace("FFW! Clean To " + valueSetTo);
                        }
                    }
                }
                //Console.Write("16 ");
                if (structWarningObj != null)
                {
                    //Console.Write("17 ");
                    SetValue(structWarningMember, structWarningParent, structWarningObj);
                }
                //Console.Write("18 ");
                if (request == "CLEAN")
                {
                    //Console.Write("19 ");
                    this.effectMemory.Remove(block);
                }
            }
            if (request == "SAVE")
            {
                //Console.Write("20 ");
                this.ManipulateObj(blockPool, "UPDATE");
            }
        }

        public float GetBuffAverage(string name)
        {
            float m = 1.0f;
            List<float> allMults = (List<float>)this.effectBuffBlocks.Select(x => x.Key.Strength(x.Value)).ToList();
            /*List<float> allMults = new List<float>();
            foreach (KeyValuePair<ModuleBuffMk2, int> buffKvp in this.effectBuffBlocks)
            {
                if (
                    (buffKvp.Key.m_AffectedBlockList[buffKvp.Value].Contains(name) && (buffKvp.Key.m_AffectedBlockListType[buffKvp.Value] == "white"))
                    ||
                    (!buffKvp.Key.m_AffectedBlockList[buffKvp.Value].Contains(name) && (buffKvp.Key.m_AffectedBlockListType[buffKvp.Value] == "black"))
                )
                {
                    allMults.Add(buffKvp.Key.Strength(buffKvp.Value));
                }
            }*/
            if (allMults.Count > 0)
            {
                m = allMults.Average();
            }
            return m;
        }

        public float GetBuffAddAverage(string name)
        {
            float a = 0.0f;
            List<float> allAdds = (List<float>)this.effectBuffBlocks.Select(x => x.Key.AddAfter(x.Value)).ToList();
            /*List<float> allAdds = new List<float>();
            foreach (KeyValuePair<ModuleBuffMk2, int> buffKvp in this.effectBuffBlocks)
            {
                //BuffBlocks.logger.Trace("FFW! Find me! ");
                foreach (string x in buffKvp.Key.m_AffectedBlockList)
                {
                    /*foreach (string y in x)
                    {
                        Console.Write(y);
                    }//
                    Console.Write(x);

                }
                if (
                    (buffKvp.Key.m_AffectedBlockList[buffKvp.Value].Contains(name) && (buffKvp.Key.m_AffectedBlockListType[buffKvp.Value] == "white"))
                    ||
                    (!buffKvp.Key.m_AffectedBlockList[buffKvp.Value].Contains(name) && (buffKvp.Key.m_AffectedBlockListType[buffKvp.Value] == "black"))
                )
                {
                    allAdds.Add(buffKvp.Key.Strength(buffKvp.Value));
                }
            }*/
            if (allAdds.Count > 0)
            {
                a = allAdds.Average();
            }
            return a;
        }
        
        public float GetAverages(string name)
        {
            return GetBuffAverage(name) + GetBuffAddAverage(name);
        }

        public void AddBuff(ModuleBuffMk2 buff, int i)
        {
            this.effectBuffBlocks.Add(buff, i);
        }

        public void RemoveBuff(ModuleBuffMk2 buff)
        {
            this.effectBuffBlocks.Remove(buff);
        }
    }
}
