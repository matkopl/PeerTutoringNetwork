import { CalendarBase } from './CalendarBase.js';
export class Calendar extends CalendarBase {
    // TODO singleton Calendar
    static instance;
    constructor(selector, events) {
        if (Calendar.instance) {
            return Calendar.instance;
        }
        super(selector, events);
        Calendar.instance = this;
    }
    openDay(el) {
        let details, arrow;
        const dayNumber = +el.querySelectorAll('.day-number')[0].innerText || +el.querySelectorAll('.day-number')[0].textContent;
        const day = this.current.clone().date(dayNumber);
        const currentOpened = document.querySelector('.details');

        if (currentOpened && currentOpened.parentNode === el.parentNode) {
            details = currentOpened;
            arrow = document.querySelector('.arrow');
        } else {
            if (currentOpened) {
                currentOpened.parentNode.removeChild(currentOpened);
            }
            details = this.createElement('div', 'details in');
            arrow = this.createElement('div', 'arrow');
            details.appendChild(arrow);
            el.parentNode.appendChild(details);
        }

        const todaysEvents = this.events.reduce((memo, ev) => {
            if (ev.date.isSame(day, 'day')) {
                memo.push(ev);
            }
            return memo;
        }, []);

        this.renderEvents(todaysEvents, details);

        const existingButtons = details.querySelectorAll('button');
        existingButtons.forEach(button => button.remove());

        const button1 = this.createElement('button', 'button-class', 'Add appointment');
        const button2 = this.createElement('button', 'button-class', 'Delete appointment');

        button1.addEventListener('click', () => {
            window.location.href = '/Appointments/Create';
        });

        button2.addEventListener('click', () => {
            const selectedEvent = document.querySelector('.event.selected');
            if (selectedEvent) {
                const eventName = selectedEvent.querySelector('span').innerText;
                fetch('/Appointments/Delete', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ eventName: eventName })
                }).then(response => {
                    if (response.ok) {
                        selectedEvent.remove();
                    } else {
                        alert('Failed to delete the appointment.');
                    }
                });
            } else {
                alert('Please select an appointment to delete.');
            }
        });

        details.appendChild(button1);
        details.appendChild(button2);

        arrow.style.left = el.offsetLeft - el.parentNode.offsetLeft + 27 + 'px';
    }

    renderEvents(events, ele) {
        const currentWrapper = ele.querySelector('.events');
        const wrapper = this.createElement('div', 'events in' + (currentWrapper ? ' new' : ''));

        events.forEach(ev => {
            const div = this.createElement('div', 'event');
            const square = this.createElement('div', 'event-category ' + ev.color);
            const span = this.createElement('span', '', ev.eventName);
            div.appendChild(square);
            div.appendChild(span);
            wrapper.appendChild(div);

            div.addEventListener('click', function() {
                document.querySelectorAll('.event').forEach(event => event.classList.remove('selected'));
                this.classList.add('selected');
            });
        });

        if (!events.length) {
            const div = this.createElement('div', 'event empty');
            const span = this.createElement('span', '', 'No Events');
            div.appendChild(span);
            wrapper.appendChild(div);
        }

        if (currentWrapper) {
            currentWrapper.className = 'events out';
            currentWrapper.addEventListener('webkitAnimationEnd', () => {
                currentWrapper.parentNode.removeChild(currentWrapper);
                ele.appendChild(wrapper);
            });
            currentWrapper.addEventListener('oanimationend', () => {
                currentWrapper.parentNode.removeChild(currentWrapper);
                ele.appendChild(wrapper);
            });
            currentWrapper.addEventListener('msAnimationEnd', () => {
                currentWrapper.parentNode.removeChild(currentWrapper);
                ele.appendChild(wrapper);
            });
            currentWrapper.addEventListener('animationend', () => {
                currentWrapper.parentNode.removeChild(currentWrapper);
                ele.appendChild(wrapper);
            });
        } else {
            ele.appendChild(wrapper);
        }
    }
}