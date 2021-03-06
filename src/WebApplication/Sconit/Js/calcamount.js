function CalCulateRowAmountBase(obj, objName, adjustOption, unitPriceField, qtyField, discountField, discountRateField, amountField, amountAfterDiscountField, totalDiscountField, totalDiscountRateField, totalAmountField, totalAmountAfterDiscountField, isBlank) {
    var objId = $(obj).attr("id");
    var parentId = objId.substring(0, objId.length - objName.length);
    var qtyId = "#" + parentId + qtyField;
    var amountId = "#" + parentId + amountField;
    var unitPriceId = "#" + parentId + unitPriceField;
    var amountAfterDiscountId = "#" + parentId + amountAfterDiscountField;
    var discountId = "#" + parentId + discountField;
    var discountRateId = "#" + parentId + discountRateField;

    if (objName == "tbQty") {
        $(amountId).attr('value', ($(unitPriceId).val() * $(qtyId).val()).toFixed(2));
    }

    if (adjustOption == "BaseOnDiscountRate") {
        //根据折扣率调整
        if (!isNaN($(qtyId).val()) && !isNaN($(discountRateId).val())) {
            if ($(qtyId).val() != 0) {
                var orgAmount;
                if (!isNaN($(amountId).val()) && $(amountId).val() != 0) {
                    orgAmount = ($(amountId).val() * 1.0).toFixed(2);
                } else {
                    orgAmount = ($(unitPriceId).val() * $(qtyId).val()).toFixed(2);
                }
                var discount = (orgAmount * $(discountRateId).val() / 100).toFixed(2);
                var amount = (orgAmount - discount).toFixed(2);
                $(discountId).attr('value', discount);
                $(amountAfterDiscountId).attr('value', amount);
            } else {
                $(discountId).attr('value', 0);
                $(amountAfterDiscountId).attr('value', 0);
            }
        }
    } else if (adjustOption == "BaseOnDiscount") {
        //根据折扣金额调整
        if (!isNaN($(qtyId).val()) && !isNaN($(discountId).val())) {
            if ($(qtyId).val() != 0) {
                var orgAmount;
                if (!isNaN($(amountId).val()) && $(amountId).val() != 0) {
                    orgAmount = ($(amountId).val() * 1.0).toFixed(2);
                } else {
                    orgAmount = ($(unitPriceId).val() * $(qtyId).val()).toFixed(2);
                }
                var amount = (orgAmount - $(discountId).val()).toFixed(2);
                var discountRate = ((1 - (amount / orgAmount)) * 100).toFixed(2);
                $(discountRateId).attr('value', discountRate);
                $(amountAfterDiscountId).attr('value', amount);
            } else {
                $(discountRateId).attr('value', '0');
                $(amountAfterDiscountId).attr('value', '0');
            }
        }
    }
    //行金额的新值赋给新值
    var newAmount = $(amountAfterDiscountId).attr('value');
    var oldAmount = $(amountAfterDiscountId).attr('oldValue');
    $(amountAfterDiscountId).attr('oldValue', newAmount);
    if (totalDiscountField != null && totalDiscountRateField != null && totalAmountField != null) {
        //调整总金额
        if (isNaN(newAmount)) {
            newAmount = 0;
        }
        if (isNaN(oldAmount)) {
            oldAmount = 0;
        }
        if (!isBlank) {
            CalCulateTotalAmount("BaseOnRowAmount", totalDiscountField, totalDiscountRateField, totalAmountField, totalAmountAfterDiscountField, (newAmount - oldAmount))
        }
    }

}






function CalCulateRowAmount(obj, objName, adjustOption, unitPriceField, qtyField, discountField, discountRateField, amountField, totalDiscountField, totalDiscountRateField, totalAmountField,totalAmountAfterDiscountField,isBlank) 
    {
		    var objId = $(obj).attr("id");
		    var parentId = objId.substring(0, objId.length - objName.length);
	        var qtyId = "#" + parentId + qtyField;
		    var amountId = "#" + parentId + amountField;
		    var unitPriceId = "#" + parentId + unitPriceField;
		    var discountId = "#" + parentId + discountField;
		    var discountRateId = "#" + parentId + discountRateField;
		    
		    if (adjustOption == "BaseOnDiscountRate") 
		    {
		    	//根据折扣率调整
	            if (!isNaN($(qtyId).val()) && !isNaN($(discountRateId).val())) 
	            {
			       if($(qtyId).val() != 0){
			            var orgAmount = ($(unitPriceId).val() * $(qtyId).val()).toFixed(2);
		    	        var discount = (orgAmount * $(discountRateId).val() / 100).toFixed(2);
                        var amount = (orgAmount - discount).toFixed(2);
                        $(discountId).attr('value', discount);
			            $(amountId).attr('value', amount);
			        }else{
			            $(discountId).attr('value', 0);
			            $(amountId).attr('value', 0);
			        }
                }
		    }else if (adjustOption == "BaseOnDiscount") 
		    {
	            //根据折扣金额调整
			    if (!isNaN($(qtyId).val()) && !isNaN($(discountId).val())) 
			    {
				    if($(qtyId).val() != 0)
				    {
				        var orgAmount = ($(unitPriceId).val() * $(qtyId).val()).toFixed(2);
				        var amount = (orgAmount - $(discountId).val()).toFixed(2);
				        var discountRate = ((1 - (amount / orgAmount)) * 100).toFixed(2);
			    	    $(discountRateId).attr('value', discountRate);
			    	    $(amountId).attr('value', amount);
			    	}else{
			    	    $(discountRateId).attr('value', '0');
			    	    $(amountId).attr('value', '0');
			    	}
			    }
		    }
		    //行金额的新值赋给新值
		    var newAmount = $(amountId).attr('value');
	    	var oldAmount = $(amountId).attr('oldValue');
	    	$(amountId).attr('oldValue', newAmount);
	    	if (totalDiscountField != null && totalDiscountRateField != null && totalAmountField != null) 
	    	{
			    //调整总金额
			    if (isNaN(newAmount)) {
		    	    newAmount = 0;
		    	}
		    	if (isNaN(oldAmount)) {
		    	    oldAmount = 0;
		    	}
		    	if(!isBlank)
		    	{
			        CalCulateTotalAmount("BaseOnRowAmount", totalDiscountField, totalDiscountRateField, totalAmountField,totalAmountAfterDiscountField,(newAmount - oldAmount))
		        }
		    }
	   
     }
    
function CalCulateTotalAmount(adjustOption, totalDiscountField, totalDiscountRateField, totalAmountField, totalAmountAfterDiscountField, rowAmountDiff) 
    {
	    if (adjustOption == "BaseOnRowAmount")
	    {
		    //根据行金额调整
		    var discountRate = $(totalDiscountRateField).val();
		    
		    if (isNaN(discountRate) || discountRate==null || discountRate=="")
		    {
			    discountRate = 0;
			}
			var totalAmount = $(totalAmountField).val();
			if (isNaN(totalAmount) || totalAmount==null || totalAmount=="") 
			{
			    totalAmount = 0;
			}
			
	        totalAmount = parseFloat(totalAmount) + parseFloat(rowAmountDiff);
		    var discount = (totalAmount * discountRate/100).toFixed(2);
		    $(totalAmountField).attr('value', totalAmount.toFixed(2));
			$(totalAmountAfterDiscountField).attr('value', (totalAmount-discount).toFixed(2));
			$(totalDiscountField).attr('value', discount);
		   
		}else if (adjustOption == "BaseOnDiscountRate") 
		{
			//根据折扣率调整
			if (!isNaN($(totalDiscountRateField).val()) && !isNaN($(totalAmountField).val())) 
			{
				var totalAmount = $(totalAmountField).val();
				var discount = (totalAmount * $(totalDiscountRateField).val()/100).toFixed(2);
				totalAmount -= discount;
				$(totalDiscountField).attr('value', discount);
				$(totalAmountAfterDiscountField).attr('value', totalAmount.toFixed(2));
			}
		} else if (adjustOption == "BaseOnDiscount")
		{
			//根据折扣金额调整
			if (!isNaN($(totalDiscountField).val())) 
			{
				var discount = $(totalDiscountField).val();
			    var totalAmount = $(totalAmountField).val();
			    if (isNaN(totalAmount))
			    {	
			        totalAmount = 0;
			    }
			    var discountRate = (100 * discount / totalAmount).toFixed(2);
			    totalAmount = totalAmount - discount;
			    $(totalAmountAfterDiscountField).attr('value', totalAmount.toFixed(2));
		    	$(totalDiscountRateField).attr('value', discountRate);
			}
		 }
	}