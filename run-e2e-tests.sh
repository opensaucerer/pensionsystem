#!/bin/bash

# Configuration
API_URL="http://localhost:8080/api"
UNIQUE_ID=$(date +%s)
EMAIL="e2e.tester.$UNIQUE_ID@example.com"
EMAIL_UPDATED="e2e.updated.$UNIQUE_ID@example.com"

# Colors for output
GREEN='\033[0;32m'
RED='\033[0;31m'
NC='\033[0m' # No Color

echo -e "\n${GREEN}=== PENSION SYSTEM END-TO-END TEST SUITE ===${NC}\n"

# -------------------------------------------------------------------------------------
# 1. CREATE MEMBER
# -------------------------------------------------------------------------------------
echo "1. Testing Member Registration..."
CREATE_RESPONSE=$(curl -s -X POST $API_URL/members \
  -H "Content-Type: application/json" \
  -d "{\"firstName\":\"Jane\",\"lastName\":\"Doe\",\"dateOfBirth\":\"1985-06-15\",\"email\":\"$EMAIL\",\"phoneNumber\":\"+2348012345678\"}")

MEMBER_ID=$(echo "$CREATE_RESPONSE" | python3 -c "import sys,json; data=json.load(sys.stdin); print(data.get('id', ''))" 2>/dev/null)

if [ -z "$MEMBER_ID" ]; then
    echo -e "${RED}❌ FAILED: Could not create member.${NC}"
    echo "Response: $CREATE_RESPONSE"
    exit 1
else
    echo -e "${GREEN}✅ PASSED: Member created successfully (ID: $MEMBER_ID).${NC}"
fi

# -------------------------------------------------------------------------------------
# 2. VALIDATION CHECK (UNDERAGE)
# -------------------------------------------------------------------------------------
echo -e "\n2. Testing Underage Member Validation (Should Fail)..."
UNDERAGE_RESPONSE=$(curl -s -w "\n%{http_code}" -X POST $API_URL/members \
  -H "Content-Type: application/json" \
  -d '{"firstName":"Child","lastName":"Test","dateOfBirth":"2015-01-01","email":"child@test.com","phoneNumber":"+2348000000000"}')

STATUS_CODE=$(echo "$UNDERAGE_RESPONSE" | tail -n 1)
if [ "$STATUS_CODE" -eq 400 ]; then
    echo -e "${GREEN}✅ PASSED: System successfully blocked underage member (400 Bad Request).${NC}"
else
    echo -e "${RED}❌ FAILED: Underage member was not properly blocked! Status: $STATUS_CODE${NC}"
    exit 1
fi

# -------------------------------------------------------------------------------------
# 3. GET MEMBER
# -------------------------------------------------------------------------------------
echo -e "\n3. Testing Get Member Profile..."
GET_RESPONSE=$(curl -s -o /dev/null -w "%{http_code}" $API_URL/members/$MEMBER_ID)
if [ "$GET_RESPONSE" -eq 200 ]; then
    echo -e "${GREEN}✅ PASSED: Member profile retrieved successfully.${NC}"
else
    echo -e "${RED}❌ FAILED: Could not retrieve member profile.${NC}"
    exit 1
fi

# -------------------------------------------------------------------------------------
# 4. UPDATE MEMBER
# -------------------------------------------------------------------------------------
echo -e "\n4. Testing Update Member Profile..."
UPDATE_RESPONSE=$(curl -s -o /dev/null -w "%{http_code}" -X PUT $API_URL/members/$MEMBER_ID \
  -H "Content-Type: application/json" \
  -d "{\"id\":\"$MEMBER_ID\",\"firstName\":\"Jane\",\"lastName\":\"Smith\",\"dateOfBirth\":\"1985-06-15\",\"email\":\"$EMAIL_UPDATED\",\"phoneNumber\":\"+2348012345678\"}")

if [ "$UPDATE_RESPONSE" -eq 200 ] || [ "$UPDATE_RESPONSE" -eq 204 ]; then
    echo -e "${GREEN}✅ PASSED: Member profile updated successfully.${NC}"
else
    echo -e "${RED}❌ FAILED: Could not update member profile. Status: $UPDATE_RESPONSE${NC}"
    exit 1
fi

# -------------------------------------------------------------------------------------
# 5. ADD MONTHLY CONTRIBUTION
# -------------------------------------------------------------------------------------
# Type 1 = Monthly
echo -e "\n5. Testing Mandatory Monthly Contribution..."
CONTRIB_1_RESPONSE=$(curl -s -o /dev/null -w "%{http_code}" -X POST $API_URL/contributions \
  -H "Content-Type: application/json" \
  -d "{\"memberId\":\"$MEMBER_ID\",\"type\":1,\"amount\":50000.00,\"contributionDate\":\"2026-01-01T00:00:00\",\"referenceNumber\":\"REF-M1-$UNIQUE_ID\"}")

if [ "$CONTRIB_1_RESPONSE" -eq 201 ]; then
    echo -e "${GREEN}✅ PASSED: Monthly contribution processed.${NC}"
else
    echo -e "${RED}❌ FAILED: Monthly contribution failed. Status: $CONTRIB_1_RESPONSE${NC}"
    exit 1
fi

# -------------------------------------------------------------------------------------
# 6. TEST DUPLICATE MONTHLY CONTRIBUTION (MUST FAIL)
# -------------------------------------------------------------------------------------
echo -e "\n6. Testing Duplicate Monthly Contribution (Should Fail)..."
DUP_RESPONSE=$(curl -s -w "\n%{http_code}" -X POST $API_URL/contributions \
  -H "Content-Type: application/json" \
  -d "{\"memberId\":\"$MEMBER_ID\",\"type\":1,\"amount\":50000.00,\"contributionDate\":\"2026-01-15T00:00:00\",\"referenceNumber\":\"REF-DUP-$UNIQUE_ID\"}")

DUP_STATUS=$(echo "$DUP_RESPONSE" | tail -n 1)
if [ "$DUP_STATUS" -eq 400 ]; then
    echo -e "${GREEN}✅ PASSED: System blocked duplicate monthly contribution correctly.${NC}"
else
    echo -e "${RED}❌ FAILED: System allowed duplicate monthly contribution! Status: $DUP_STATUS${NC}"
    exit 1
fi

# -------------------------------------------------------------------------------------
# 7. ADD VOLUNTARY CONTRIBUTION
# -------------------------------------------------------------------------------------
# Type 2 = Voluntary
echo -e "\n7. Testing Voluntary Contribution..."
VOL_RESPONSE=$(curl -s -o /dev/null -w "%{http_code}" -X POST $API_URL/contributions \
  -H "Content-Type: application/json" \
  -d "{\"memberId\":\"$MEMBER_ID\",\"type\":2,\"amount\":25000.00,\"contributionDate\":\"2026-01-20T00:00:00\",\"referenceNumber\":\"REF-V1-$UNIQUE_ID\"}")

if [ "$VOL_RESPONSE" -eq 201 ]; then
    echo -e "${GREEN}✅ PASSED: Voluntary contribution processed (bypassed monthly limit).${NC}"
else
    echo -e "${RED}❌ FAILED: Voluntary contribution failed. Status: $VOL_RESPONSE${NC}"
    exit 1
fi

# -------------------------------------------------------------------------------------
# 8. TEST BENEFIT WITHDRAWAL (INSUFFICIENT CONDITIONS)
# -------------------------------------------------------------------------------------
echo -e "\n8. Testing Early Benefit Withdrawal (Should Fail - < 12 Months)..."
BENEFIT_FAIL_RESPONSE=$(curl -s -w "\n%{http_code}" -X POST $API_URL/benefits/withdraw \
  -H "Content-Type: application/json" \
  -d "{\"memberId\":\"$MEMBER_ID\",\"amount\":10000.00,\"type\":1}")

BENEFIT_STATUS=$(echo "$BENEFIT_FAIL_RESPONSE" | tail -n 1)
if [ "$BENEFIT_STATUS" -eq 400 ]; then
    echo -e "${GREEN}✅ PASSED: System correctly blocked withdrawal (Member hasn't met 12 month rule).${NC}"
else
    echo -e "${RED}❌ FAILED: System allowed withdrawal prematurely! Status: $BENEFIT_STATUS${NC}"
    exit 1
fi

# -------------------------------------------------------------------------------------
# 9. GET MEMBER STATEMENT
# -------------------------------------------------------------------------------------
echo -e "\n9. Testing Comprehensive Member Statement..."
STATEMENT_RESPONSE=$(curl -s -X GET $API_URL/members/$MEMBER_ID/statement)
BALANCE=$(echo "$STATEMENT_RESPONSE" | python3 -c "import sys,json; data=json.load(sys.stdin); print(data.get('currentBalance', ''))" 2>/dev/null)

if [ "$BALANCE" == "75000.0" ]; then
    echo -e "${GREEN}✅ PASSED: Statement retrieved. Exact balance calculated (75,000.00).${NC}"
else
    echo -e "${RED}❌ FAILED: Balance mismatch. Expected 75000.0, got $BALANCE${NC}"
    exit 1
fi

# -------------------------------------------------------------------------------------
# 10. SOFT DELETE MEMBER
# -------------------------------------------------------------------------------------
echo -e "\n10. Testing Member Soft Deletion..."
DELETE_RESPONSE=$(curl -s -o /dev/null -w "%{http_code}" -X DELETE $API_URL/members/$MEMBER_ID)

if [ "$DELETE_RESPONSE" -eq 204 ]; then
    echo -e "${GREEN}✅ PASSED: Member successfully soft-deleted.${NC}"
else
    echo -e "${RED}❌ FAILED: Deletion failed. Status: $DELETE_RESPONSE${NC}"
    exit 1
fi

# Verify Deletion
VERIFY_DELETE=$(curl -s -o /dev/null -w "%{http_code}" $API_URL/members/$MEMBER_ID)
if [ "$VERIFY_DELETE" -eq 404 ]; then
    echo -e "${GREEN}✅ PASSED: System returns 404 Not Found for soft-deleted member.${NC}"
else
    echo -e "${RED}❌ FAILED: Member still accessible after soft delete! Status: $VERIFY_DELETE${NC}"
    exit 1
fi

echo -e "\n${GREEN}===================================================${NC}"
echo -e "${GREEN}  🎉 ALL E2E TESTS EXECUTED AND PASSED FLAWLESSLY 🎉  ${NC}"
echo -e "${GREEN}===================================================${NC}\n"
