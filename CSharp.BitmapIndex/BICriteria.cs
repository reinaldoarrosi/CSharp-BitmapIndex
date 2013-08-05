using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitmapIndex
{
    public class BICriteria 
    {	
	    public enum Operator 
        {
		    EQUALS = 0,
		    EQUALS_OR_EMPTY = 1,
		    NOT_EQUALS = 2,
		    NOT_EQUALS_OR_EMPTY = 3,
            EMPTY_ONLY = 6,
		    OR = 4,
		    AND = 5
	    }
	
	    private BIKey _key;
	    private Operator _criteriaOperator;
	    private BICriteria _left;
	    private BICriteria _right;

        private BICriteria(BIKey key, Operator criteriaOperator)
        {
		    _key = key;
            _criteriaOperator = criteriaOperator;
	    }

        private BICriteria(BICriteria left, BICriteria right, Operator criteriaOperator)
        {
		    _left = left;
		    _right = right;
            _criteriaOperator = criteriaOperator;
	    }

        public Operator CriteriaOperator
        {
            get
            {
                return _criteriaOperator;
            }
	    }
	
	    public BIKey Key 
        {
            get
            {
                return _key;
            }
	    }
	
	    public BICriteria LeftCriteria
        {
            get
            {
		        return _left;
            }
	    }
	
	    public BICriteria RightCriteria
        {
            get
            {
                return _right;
            }
	    }
	
	    public BICriteria or(BICriteria criteria) 
        {
		    return new BICriteria(this, criteria, Operator.OR);
	    }
	
	    public BICriteria and(BICriteria criteria) 
        {
		    return new BICriteria(this, criteria, Operator.AND);
	    }
	
	    public BICriteria andEquals(BIKey key) 
        {
		    return and(new BICriteria(key, Operator.EQUALS));
	    }
	
	    public BICriteria andNotEquals(BIKey key) {
		    return and(new BICriteria(key, Operator.NOT_EQUALS));
	    }
	
	    public BICriteria andEqualsOrEmpty(BIKey key) {
		    return and(new BICriteria(key, Operator.EQUALS_OR_EMPTY));
	    }
	
	    public BICriteria andNotEqualsOrEmpty(BIKey key) {
		    return and(new BICriteria(key, Operator.NOT_EQUALS_OR_EMPTY));
	    }

        public BICriteria andEmptyOnly(int group)
        {
            return and(new BICriteria(new BIKey(group, null), Operator.EMPTY_ONLY));
        }

        public BICriteria andEmptyOnly(BIKey.BIGroup group)
        {
            return and(new BICriteria(new BIKey(group, null), Operator.EMPTY_ONLY));
        }

	    public BICriteria orEquals(BIKey key) {
		    return or(new BICriteria(key, Operator.EQUALS));
	    }
	
	    public BICriteria orNotEquals(BIKey key) {
		    return or(new BICriteria(key, Operator.NOT_EQUALS));
	    }
	
	    public BICriteria orEqualsOrEmpty(BIKey key) {
		    return or(new BICriteria(key, Operator.EQUALS_OR_EMPTY));
	    }
	
	    public BICriteria orNotEqualsOrEmpty(BIKey key) {
		    return or(new BICriteria(key, Operator.NOT_EQUALS_OR_EMPTY));
	    }

        public BICriteria orEmptyOnly(int group)
        {
            return or(new BICriteria(new BIKey(group, null), Operator.EMPTY_ONLY));
        }

        public BICriteria orEmptyOnly(BIKey.BIGroup group)
        {
            return or(new BICriteria(new BIKey(group, null), Operator.EMPTY_ONLY));
        }
	
	    public static BICriteria equals(BIKey key) {
		    return new BICriteria(key, Operator.EQUALS);
	    }
	
	    public static BICriteria notEquals(BIKey key) {
		    return new BICriteria(key, Operator.NOT_EQUALS);
	    }
	
	    public static BICriteria equalsOrEmpty(BIKey key) {
		    return new BICriteria(key, Operator.EQUALS_OR_EMPTY);
	    }
	
	    public static BICriteria notEqualsOrEmpty(BIKey key) {
		    return new BICriteria(key, Operator.NOT_EQUALS_OR_EMPTY);
	    }

        public static BICriteria emptyOnly(int group)
        {
            return new BICriteria(new BIKey(group, null), Operator.EMPTY_ONLY);
        }

        public static BICriteria emptyOnly(BIKey.BIGroup group)
        {
            return new BICriteria(new BIKey(group, null), Operator.EMPTY_ONLY);
        }
    }
}
