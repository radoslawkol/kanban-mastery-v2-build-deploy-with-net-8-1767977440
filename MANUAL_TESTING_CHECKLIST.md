# Kanban Application - Manual Testing Checklist

## 1. REGISTRATION & LOGIN

### 1.1 Registration Flow

- **Test**: Register a new account
    - Email: test@example.pl
    - Username: testUser
    - Password: 12345678Ab!
    - Confirm Password: 12345678Ab!

**Observations:**

- Form validation works: Yes
- Success message displayed: Yes
- Redirected to login: Yes
- Any UI issues/glitches: No

### 1.2 Login Flow

- **Test**: Log in with registered account
    - Email/Username: test@example.pl
    - Password: 12345678Ab!

**Observations:**

- Form feels smooth/responsive: Yes
- Success feedback: Yes
- Redirected to dashboard/boards: Yes
- Session persists on page refresh: Yes
- Any UI issues/glitches: No

### 1.3 Failed Login Attempts

- **Test**: Log in with incorrect password
    - Credentials tried: 12345678Ab\*

**Error Message: "Unauthorize"
**Helpful/Clear:** Yes
**Notes:\*\* Error message could be more specific e.g Invalid credenitals

---

## 2. BOARD CREATION

### 2.1 Create First Board

- **Test**: Create a new board from authenticated state

**Board Details:**

- Board Name: Test Board

**Observations:**

- Form accessible: Yes
- Input fields responsive: Yes
- Submit button responsive: Yes
- Success feedback (confirmation/toast): Yes
- Redirected to board: Yes
- Board appears in list: Yes
- Any UI issues/glitches: Works perfect

---

## 3. COLUMN MANAGEMENT

### 3.1 Add Columns

- **Test**: Add columns to the board

**Column 1:**

- Name: To Do
- Added successfully: Yes
- Visual feedback received: Yes, message and created without refreshing

**Column 2:**

- Name: In Progress
- Added successfully: Yes
- Visual feedback received: Yes, message and created without refreshing

**Column 3:**

- Name: Done
- Added successfully: Yes
- Visual feedback received: Yes, message and created without refreshing

**Observations:**

- Add column button is obvious: Yes
- Form/modal UX feels natural: Yes
- Columns render in correct order: Yes
- Any lag or delays: Not observed

### 3.2 Edit Column

- **Test**: Rename a column

**Original Name:** To Do
**New Name:** TODO
**Success:** Yes
**Feedback:** Column created successfully

### 3.3 Delete Column

- **Test**: Delete a column

**Column Deleted:** Yes
**Confirmation required:** Yes
**Success:** Yes
Note: Column remove right-away (don't need to refresh)

---

## 4. CARD MANAGEMENT

### 4.1 Create Cards

- **Test**: Create multiple cards in different columns

**Card 1:**

- Column: TODO
- Title: Prepare meal
- Assigned To: Unassigned
- Created successfully: Yes
- Appears in correct column: Yes

**Card 2:**

- Column: In Progress
- Title: Do homework
- Assigned To: Unassigned
- Created successfully: Yes
- Appears in correct column: Yes

**Card 3:**

- Column: TODO
- Title: Take shower
- Assigned To: Unassigned
- Created successfully: Yes
- Appears in correct column: Yes

**Observations:**

- Card creation form is intuitive: Yes
- Input validation works: Yes
- Visual feedback on creation: Card visible right-away
- Any lag: No

---

## 5. DRAG-AND-DROP FUNCTIONALITY

### 5.1 Drag Card Between Columns

- **Test**: Move a card from one column to another

**Card Title:** Prepare meal
**From Column:** TODO
**To Column:** In Progress

**Observations:**

- Card is draggable (cursor changes): Yes
- Drag feels smooth/responsive: Yes
- Drop zone is obvious: Yes
- Card lands in correct position: Yes
- Update persists on refresh: Yes

### 5.2 Reorder Cards Within Column

- **Test**: Drag a card to reorder within same column

**Card Moved:** Prepare Meal
**New Position:** After Do homework in Progres card
**Reorder successful:** Yes
**Note**: After refreshing order in columns is not correct, because order of all columns card are not updated in db

---

## 6. USER INVITATION & COLLABORATION

### 6.1 Invite Another User

- **Test**: Invite a second user to the board (Owner account)

**Invited User Email:** ol@example.pl
**Success Message:** Yes
**Invite Sent Successfully:** Yes

### 6.2 Accept Invitation (New User)

- **Test**: Log in as the invited user and verify access

**Invited User Email:** ol@example.pl

**Observations:**

- Invited user can log in: Yes
- Board appears in user's board list: Yes
- Can see all columns and cards: Yes

### 6.3 Verify Permissions

- **Test**: Invited user attempts actions

**Can Create Card:** Yes
**Can Edit Card:** Yes
**Can Delete Card:** Yes / No  
**Can Create Column:** Yes
**Can Edit Column:** Yes
**Can Delete Column:** Yes
**Can Invite Others:** No  
**Can Delete Board:** No
