# Kanban Application - Manual Testing Checklist

## 1. REGISTRATION & LOGIN

### 1.1 Registration Flow

- **Test**: Register a new account
    - Email: **\*\***\_\_\_\_**\*\***
    - Username: **\*\***\_\_\_\_**\*\***
    - Password: **\*\***\_\_\_\_**\*\***
    - Confirm Password: **\*\***\_\_\_\_**\*\***

**Observations:**

- Form validation works: Yes / No
- Error message (if applicable): **\*\***\_\_\_\_**\*\***
- Success message displayed: Yes / No
- Redirected to login: Yes / No
- Any UI issues/glitches: **\*\***\_\_\_\_**\*\***

### 1.2 Login Flow

- **Test**: Log in with registered account
    - Email/Username: **\*\***\_\_\_\_**\*\***
    - Password: **\*\***\_\_\_\_**\*\***

**Observations:**

- Form feels smooth/responsive: Yes / No
- Error handling (if applicable): **\*\***\_\_\_\_**\*\***
- Success feedback: **\*\***\_\_\_\_**\*\***
- Redirected to dashboard/boards: Yes / No
- Session persists on page refresh: Yes / No
- Any UI issues/glitches: **\*\***\_\_\_\_**\*\***

### 1.3 Failed Login Attempts

- **Test**: Log in with incorrect password
    - Credentials tried: **\*\***\_\_\_\_**\*\***

**Error Message:** **\*\***\_\_\_\_**\*\***  
**Helpful/Clear:** Yes / No  
**Notes:** **\*\***\_\_\_\_**\*\***

---

## 2. BOARD CREATION

### 2.1 Create First Board

- **Test**: Create a new board from authenticated state

**Board Details:**

- Board Name: **\*\***\_\_\_\_**\*\***
- Description (if applicable): **\*\***\_\_\_\_**\*\***

**Observations:**

- Form accessible: Yes / No
- Input fields responsive: Yes / No
- Submit button responsive: Yes / No
- Success feedback (confirmation/toast): **\*\***\_\_\_\_**\*\***
- Redirected to board: Yes / No
- Board appears in list: Yes / No
- Any UI issues/glitches: **\*\***\_\_\_\_**\*\***

---

## 3. COLUMN MANAGEMENT

### 3.1 Add Columns

- **Test**: Add columns to the board

**Column 1:**

- Name: **\*\***\_\_\_\_**\*\***
- Added successfully: Yes / No
- Visual feedback received: **\*\***\_\_\_\_**\*\***

**Column 2:**

- Name: **\*\***\_\_\_\_**\*\***
- Added successfully: Yes / No
- Visual feedback received: **\*\***\_\_\_\_**\*\***

**Column 3:**

- Name: **\*\***\_\_\_\_**\*\***
- Added successfully: Yes / No
- Visual feedback received: **\*\***\_\_\_\_**\*\***

**Observations:**

- Add column button is obvious: Yes / No
- Form/modal UX feels natural: Yes / No
- Columns render in correct order: Yes / No
- Responsive to keyboard input: Yes / No
- Any lag or delays: **\*\***\_\_\_\_**\*\***

### 3.2 Edit Column

- **Test**: Rename a column

**Original Name:** **\*\***\_\_\_\_**\*\***  
**New Name:** **\*\***\_\_\_\_**\*\***  
**Success:** Yes / No  
**Feedback:** **\*\***\_\_\_\_**\*\***

### 3.3 Delete Column

- **Test**: Delete a column

**Column Deleted:** **\*\***\_\_\_\_**\*\***  
**Confirmation required:** Yes / No  
**Success:** Yes / No  
**Any data loss issues:** **\*\***\_\_\_\_**\*\***

---

## 4. CARD MANAGEMENT

### 4.1 Create Cards

- **Test**: Create multiple cards in different columns

**Card 1:**

- Column: **\*\***\_\_\_\_**\*\***
- Title: **\*\***\_\_\_\_**\*\***
- Description: **\*\***\_\_\_\_**\*\***
- Assigned To: **\*\***\_\_\_\_**\*\***
- Created successfully: Yes / No
- Appears in correct column: Yes / No

**Card 2:**

- Column: **\*\***\_\_\_\_**\*\***
- Title: **\*\***\_\_\_\_**\*\***
- Description: **\*\***\_\_\_\_**\*\***
- Assigned To: **\*\***\_\_\_\_**\*\***
- Created successfully: Yes / No
- Appears in correct column: Yes / No

**Card 3:**

- Column: **\*\***\_\_\_\_**\*\***
- Title: **\*\***\_\_\_\_**\*\***
- Description: **\*\***\_\_\_\_**\*\***
- Assigned To: **\*\***\_\_\_\_**\*\***
- Created successfully: Yes / No
- Appears in correct column: Yes / No

**Observations:**

- Card creation form is intuitive: Yes / No
- Input validation works: Yes / No
- Visual feedback on creation: **\*\***\_\_\_\_**\*\***
- Any lag: **\*\***\_\_\_\_**\*\***

### 4.2 Edit Card

- **Test**: Edit an existing card

**Original Title:** **\*\***\_\_\_\_**\*\***  
**New Title:** **\*\***\_\_\_\_**\*\***  
**Fields Changed:** **\*\***\_\_\_\_**\*\***  
**Update Successful:** Yes / No  
**Real-time update visible:** Yes / No

### 4.3 Delete Card

- **Test**: Delete a card

**Card Deleted:** **\*\***\_\_\_\_**\*\***  
**Confirmation required:** Yes / No  
**Removed from UI:** Yes / No

---

## 5. DRAG-AND-DROP FUNCTIONALITY

### 5.1 Drag Card Between Columns

- **Test**: Move a card from one column to another

**Card Title:** **\*\***\_\_\_\_**\*\***  
**From Column:** **\*\***\_\_\_\_**\*\***  
**To Column:** **\*\***\_\_\_\_**\*\***

**Observations:**

- Card is draggable (cursor changes): Yes / No
- Drag feels smooth/responsive: Yes / No
- Drop zone is obvious: Yes / No
- Card lands in correct position: Yes / No
- Update persists on refresh: Yes / No
- Any visual glitches during drag: **\*\***\_\_\_\_**\*\***
- Latency noticeable: Yes / No (if yes, approx **\_** ms)

### 5.2 Reorder Cards Within Column

- **Test**: Drag a card to reorder within same column

**Card Moved:** **\*\***\_\_\_\_**\*\***  
**New Position:** **\*\***\_\_\_\_**\*\***  
**Reorder successful:** Yes / No

---

## 6. USER INVITATION & COLLABORATION

### 6.1 Invite Another User

- **Test**: Invite a second user to the board (Owner account)

**Invited User Email:** **\*\***\_\_\_\_**\*\***  
**Invite Method:** (dropdown / modal / other) **\*\***\_\_\_\_**\*\***  
**Success Message:** **\*\***\_\_\_\_**\*\***  
**Invite Sent Successfully:** Yes / No

### 6.2 Accept Invitation (New User)

- **Test**: Log in as the invited user and verify access

**Invited User Email:** **\*\***\_\_\_\_**\*\***  
**Invited User Password:** **\*\***\_\_\_\_**\*\***

**Observations:**

- Invited user can log in: Yes / No
- Board appears in user's board list: Yes / No
- Can see all columns and cards: Yes / No
- User role displayed correctly: **\*\***\_\_\_\_**\*\***
- Permissions respected: Yes / No

### 6.3 Verify Permissions

- **Test**: Invited user attempts actions

**Can Create Card:** Yes / No  
**Can Edit Card:** Yes / No  
**Can Delete Card:** Yes / No  
**Can Create Column:** Yes / No  
**Can Edit Column:** Yes / No  
**Can Delete Column:** Yes / No  
**Can Invite Others:** Yes / No  
**Can Delete Board:** Yes / No

**Observations:**

- Permission restrictions are clear: Yes / No
- Error messages helpful: Yes / No

---

## 7. PERFORMANCE & RESPONSIVENESS

### 7.1 Load Times

- **Test**: Measure page load times

**Dashboard Load Time:** **\*\***\_\_\_\_**\*\*** seconds  
**Board Load Time:** **\*\***\_\_\_\_**\*\*** seconds  
**Card Creation Response Time:** **\*\***\_\_\_\_**\*\*** seconds  
**Drag-and-Drop Response Time:** **\*\***\_\_\_\_**\*\*** seconds

### 7.2 Browser Performance

- **Browser:** **\*\***\_\_\_\_**\*\***
- **Console Errors:** Yes / No
    - If yes, describe: **\*\***\_\_\_\_**\*\***
- **Console Warnings:** Yes / No
    - If yes, describe: **\*\***\_\_\_\_**\*\***
- **Memory Issues:** Yes / No
- **Network Errors (Network Tab):** Yes / No

### 7.3 API Logs

- **API Running Without Errors:** Yes / No
- **Issues Found:** **\*\***\_\_\_\_**\*\***

---

## 8. UX & VISUAL POLISH

### 8.1 General Impressions

- **Intuitive:** Yes / No | Comments: **\*\***\_\_\_\_**\*\***
- **Visually Consistent:** Yes / No | Comments: **\*\***\_\_\_\_**\*\***
- **Colors/Theme:** **\*\***\_\_\_\_**\*\***
- **Typography:** **\*\***\_\_\_\_**\*\***
- **Responsive Layout:** Yes / No | Comments: **\*\***\_\_\_\_**\*\***

### 8.2 Navigation

- **Navigation Clear:** Yes / No
- **Buttons/Links Obvious:** Yes / No
- **Back Navigation Works:** Yes / No

### 8.3 Error Messages

- **Clear & Helpful:** Yes / No
- **Examples of improvements needed:** **\*\***\_\_\_\_**\*\***

### 8.4 Success Feedback

- **Confirmations Shown:** Yes / No
- **Toasts/Notifications Helpful:** Yes / No

---

## 9. EDGE CASES & ISSUES FOUND

### 9.1 Bug/Issue #1

**Description:** **\*\***\_\_\_\_**\*\***  
**Steps to Reproduce:**

1. ***
2. ***
3. ***

**Expected Behavior:** **\*\***\_\_\_\_**\*\***  
**Actual Behavior:** **\*\***\_\_\_\_**\*\***  
**Severity:** Critical / High / Medium / Low  
**Screenshot/Console Error:** **\*\***\_\_\_\_**\*\***

### 9.2 Bug/Issue #2

**Description:** **\*\***\_\_\_\_**\*\***  
**Steps to Reproduce:**

1. ***
2. ***
3. ***

**Expected Behavior:** **\*\***\_\_\_\_**\*\***  
**Actual Behavior:** **\*\***\_\_\_\_**\*\***  
**Severity:** Critical / High / Medium / Low  
**Screenshot/Console Error:** **\*\***\_\_\_\_**\*\***

### 9.3 Additional Issues

---

---

---

---

## 10. ACCEPTANCE CRITERIA SUMMARY

| Criteria                                             | Status          | Notes                    |
| ---------------------------------------------------- | --------------- | ------------------------ |
| User can register and log in                         | ☐ Pass / ☐ Fail | **\*\***\_\_\_\_**\*\*** |
| Authenticated user can create a board                | ☐ Pass / ☐ Fail | **\*\***\_\_\_\_**\*\*** |
| Owner can invite another user                        | ☐ Pass / ☐ Fail | **\*\***\_\_\_\_**\*\*** |
| Members can create, update, delete columns and cards | ☐ Pass / ☐ Fail | **\*\***\_\_\_\_**\*\*** |
| Drag-and-drop moves cards between columns            | ☐ Pass / ☐ Fail | **\*\***\_\_\_\_**\*\*** |

---

## 11. OVERALL ASSESSMENT

**Overall Quality:** Excellent / Good / Fair / Needs Work

**Recommended Improvements:**

1. ***
2. ***
3. ***
