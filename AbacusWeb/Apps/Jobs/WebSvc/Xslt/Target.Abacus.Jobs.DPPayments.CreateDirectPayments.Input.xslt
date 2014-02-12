<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="html" indent="no" encoding="UTF-8" />
    <!--
    Waqas D12009 on 17/02/2011 Updated  to add sds Filter info
		Abacus for Windows Job Service - Create DP Payments
		
		Formats the input information for display.
	-->
    <xsl:template match="Inputs">
      <xsl:choose>
        <xsl:when test="string-length(FilterGeneratePayments) &gt; 0">
          <xsl:choose>
            <xsl:when test="FilterGeneratePayments = 'False' or FilterGeneratePayments = 'false'">
              (REPORT ONLY) Payments will not be created
              <br />
            </xsl:when>
          </xsl:choose>
        </xsl:when>
      </xsl:choose>
        <xsl:choose>
            <xsl:when test="string-length(FilterServiceUserReferenceAndName) &gt; 0">
                Selected Service User: <xsl:value-of select="FilterServiceUserReferenceAndName" /><br />
            </xsl:when>
            <xsl:otherwise>
                Selected Service User: (all)<br />
            </xsl:otherwise>
        </xsl:choose>
        <xsl:choose>
            <xsl:when test="string-length(FilterBudgetHolderReferenceAndName) &gt; 0">
                Selected Budget Holder: <xsl:value-of select="FilterBudgetHolderReferenceAndName" /><br />
            </xsl:when>
            <xsl:otherwise>
                Selected Budget Holder: (all)<br />
            </xsl:otherwise>
        </xsl:choose>
      <xsl:choose>
        <xsl:when test="string-length(FilterSDS) &gt; 0">
            SDS?:
            <xsl:choose>
                <xsl:when test="FilterSDS = '-1'">
                    Yes
                </xsl:when>
                <xsl:when test="FilterSDS = '0'">
                    No
                </xsl:when>
                <xsl:otherwise>
                    (all)
                </xsl:otherwise>
            </xsl:choose>            
            <br />
        </xsl:when>
        <xsl:otherwise>
          SDS? : (all)<br />
        </xsl:otherwise>
      </xsl:choose>
        <xsl:choose>
            <xsl:when test="string-length(FilterDateTo) &gt; 0">
                Selected Date To: <xsl:value-of select="FilterDateTo" /><br />
            </xsl:when>
            <xsl:otherwise>
                Selected Date To: (all)<br />
            </xsl:otherwise>
        </xsl:choose>
        <xsl:choose>
            <xsl:when test="string-length(FilterDoNotCollectReconsideredPayments) &gt; 0">
                Do not collect payments from DP Contract Payments that are marked for reconsideration?: 
                <xsl:choose>
                    <xsl:when test="FilterDoNotCollectReconsideredPayments = 'True' or FilterDoNotCollectReconsideredPayments = 'true'">
                        Yes
                    </xsl:when>
                    <xsl:otherwise>
                        No
                    </xsl:otherwise>
                </xsl:choose>
                <br />
              </xsl:when>
              <xsl:otherwise>
                  Do not collect payments from DP Contract Payments that are marked for reconsideration?: Yes<br />
              </xsl:otherwise>
            </xsl:choose>
       <xsl:choose>
          <xsl:when test="string-length(FilterGeneratePayments) &gt; 0">
            Generate Payments?:
            <xsl:choose>
              <xsl:when test="FilterGeneratePayments = 'True' or FilterGeneratePayments = 'true'">
                Yes
              </xsl:when>
              <xsl:otherwise>
                No
              </xsl:otherwise>
            </xsl:choose>
            <br />
          </xsl:when>
          <xsl:otherwise>
            Generate Payments?: Yes<br />
          </xsl:otherwise>
       </xsl:choose>
    </xsl:template>
</xsl:stylesheet>
