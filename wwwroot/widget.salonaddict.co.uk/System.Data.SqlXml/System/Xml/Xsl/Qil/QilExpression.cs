namespace System.Xml.Xsl.Qil
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Xml;

    internal class QilExpression : QilNode
    {
        private QilNode defWSet;
        private QilNode earlBnd;
        private QilFactory factory;
        private QilNode funList;
        private QilNode gloParams;
        private QilNode gloVars;
        private QilNode isDebug;
        private QilNode rootNod;
        private QilNode wsRules;

        public QilExpression(QilNodeType nodeType, QilNode root) : this(nodeType, root, new QilFactory())
        {
        }

        public QilExpression(QilNodeType nodeType, QilNode root, QilFactory factory) : base(nodeType)
        {
            this.factory = factory;
            this.isDebug = factory.False();
            XmlWriterSettings settings = new XmlWriterSettings {
                ConformanceLevel = ConformanceLevel.Auto
            };
            this.defWSet = factory.LiteralObject(settings);
            this.wsRules = factory.LiteralObject(new List<WhitespaceRule>());
            this.gloVars = factory.GlobalVariableList();
            this.gloParams = factory.GlobalParameterList();
            this.earlBnd = factory.LiteralObject(new List<EarlyBoundInfo>());
            this.funList = factory.FunctionList();
            this.rootNod = root;
        }

        public override int Count =>
            8;

        public XmlWriterSettings DefaultWriterSettings
        {
            get => 
                ((XmlWriterSettings) ((QilLiteral) this.defWSet).Value);
            set
            {
                value.ReadOnly = true;
                ((QilLiteral) this.defWSet).Value = value;
            }
        }

        public IList<EarlyBoundInfo> EarlyBoundTypes
        {
            get => 
                ((IList<EarlyBoundInfo>) ((QilLiteral) this.earlBnd).Value);
            set
            {
                ((QilLiteral) this.earlBnd).Value = value;
            }
        }

        public QilFactory Factory
        {
            get => 
                this.factory;
            set
            {
                this.factory = value;
            }
        }

        public QilList FunctionList
        {
            get => 
                ((QilList) this.funList);
            set
            {
                this.funList = value;
            }
        }

        public QilList GlobalParameterList
        {
            get => 
                ((QilList) this.gloParams);
            set
            {
                this.gloParams = value;
            }
        }

        public QilList GlobalVariableList
        {
            get => 
                ((QilList) this.gloVars);
            set
            {
                this.gloVars = value;
            }
        }

        public bool IsDebug
        {
            get => 
                (this.isDebug.NodeType == QilNodeType.True);
            set
            {
                this.isDebug = value ? this.factory.True() : this.factory.False();
            }
        }

        public override QilNode this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return this.isDebug;

                    case 1:
                        return this.defWSet;

                    case 2:
                        return this.wsRules;

                    case 3:
                        return this.gloParams;

                    case 4:
                        return this.gloVars;

                    case 5:
                        return this.earlBnd;

                    case 6:
                        return this.funList;

                    case 7:
                        return this.rootNod;
                }
                throw new IndexOutOfRangeException();
            }
            set
            {
                switch (index)
                {
                    case 0:
                        this.isDebug = value;
                        return;

                    case 1:
                        this.defWSet = value;
                        return;

                    case 2:
                        this.wsRules = value;
                        return;

                    case 3:
                        this.gloParams = value;
                        return;

                    case 4:
                        this.gloVars = value;
                        return;

                    case 5:
                        this.earlBnd = value;
                        return;

                    case 6:
                        this.funList = value;
                        return;

                    case 7:
                        this.rootNod = value;
                        return;
                }
                throw new IndexOutOfRangeException();
            }
        }

        public QilNode Root
        {
            get => 
                this.rootNod;
            set
            {
                this.rootNod = value;
            }
        }

        public IList<WhitespaceRule> WhitespaceRules
        {
            get => 
                ((IList<WhitespaceRule>) ((QilLiteral) this.wsRules).Value);
            set
            {
                ((QilLiteral) this.wsRules).Value = value;
            }
        }
    }
}

